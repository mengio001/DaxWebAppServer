using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using System.Security.Claims;
using Yarp.ReverseProxy.Transforms;
using System.Text.Json.Serialization;
using IdentityModel.Client;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Text.Json;

namespace QuizTowerPlatform.Bff
{
    public class Program
    {
        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false, true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", true)
            .AddEnvironmentVariables()
            .Build();

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            if (!builder.Environment.IsDevelopment() /*&& !builder.Environment.IsTest()*/)
            {
                // Clear all existing logging providers
                builder.Logging.ClearProviders();
            }

            ConfigureServices(builder.Services, builder.Configuration);

            var app = builder.Build();

            Configure(app, app.Environment);

            app.Run();
        }

        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllersWithViews();

            const string bffCookieScheme = "BFFCookieScheme";
            const string bffOpenIdConnectChallengeScheme = "BFFChallengeScheme";
            var idpAuthority = configuration["Application:IdPAuthority"];

            services.AddHttpClient("IDPClient", client =>
            {
                client.BaseAddress = idpAuthority == null ? null : new Uri(idpAuthority);
            });

            // Add authentication and authorization services
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = bffCookieScheme;
                options.DefaultChallengeScheme = bffOpenIdConnectChallengeScheme;
                options.DefaultSignOutScheme = bffOpenIdConnectChallengeScheme;
            })
            .AddCookie(bffCookieScheme, options =>
            {
                options.Cookie.Name = bffCookieScheme;
                options.Cookie.SameSite = SameSiteMode.Strict;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                options.SlidingExpiration = true;
                //options.LoginPath = "/account/login";
                options.AccessDeniedPath = "/error";
                options.Events.OnRedirectToLogin = (context) =>
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Task.CompletedTask;
                };
            })
            .AddOpenIdConnect(bffOpenIdConnectChallengeScheme, options =>
            {
                options.SignInScheme = bffCookieScheme;
                options.Authority = $"{idpAuthority}";
                options.ClientId = configuration["Application:OidcClientId"];
                options.ClientSecret = configuration["Application:OidcClientSecret"];
                options.ResponseType = "code";
                //options.ResponseMode = "query";
                options.SaveTokens = true;
                //// Specify where the user will be redirected after a successful login
                options.CallbackPath = "/signin-oidc";
                //// Specify where the user will be redirected after logout || "/signout-callback-oidc" || "/signout-oidc"
                options.SignedOutCallbackPath = "/signout-callback-oidc";
                options.RemoteSignOutPath = "/signout-oidc";
                options.SignedOutRedirectUri = "/";
                options.GetClaimsFromUserInfoEndpoint = true; // Note: This flag handles automatically, get all claims from IdentityServer.
                options.Scope.Clear();
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.Scope.Add("towerofquizzesapi.read");
                options.Scope.Add("towerofquizzesapi.write");
                options.Scope.Add("towerofquizzesbffapi.read");
                options.Scope.Add("towerofquizzesbffapi.write");
                options.Scope.Add("offline_access");
                options.Scope.Add("roles");
                options.Scope.Add("country");
                options.ClaimActions.MapJsonKey("role", "role");
                options.ClaimActions.MapUniqueJsonKey("country", "country");
                options.TokenValidationParameters = new()
                {
                    ValidateIssuer = true,
                    ValidIssuer = $"{idpAuthority}",
                    NameClaimType = "given_name",
                    RoleClaimType = "role",
                };
                options.Events = new OpenIdConnectEvents
                {
                    OnSignedOutCallbackRedirect = context =>
                    {
                        context.Response.Redirect(context.Options.SignedOutRedirectUri);
                        context.HandleResponse();
                        return Task.CompletedTask;
                    }
                };
            });

            services.AddAuthorization();
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(60);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            // YARP reverse proxy services - src: https://microsoft.github.io/reverse-proxy/articles/getting-started.html
            services.AddReverseProxy()
                .LoadFromConfig(Configuration.GetSection("ReverseProxy"))
                .AddTransforms(builderContext =>
                {
                    builderContext.AddRequestTransform(async transformContext =>
                    {
                        var accessToken = await transformContext.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);

                        if (string.IsNullOrEmpty(accessToken) || await TokenIsExpired(accessToken, configuration))
                        {
                            var refreshToken = await transformContext.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);
                            if (!string.IsNullOrEmpty(refreshToken))
                            {
                                // Refresh the access token
                                var newTokens = await RefreshAccessTokenAsync(refreshToken, configuration);

                                if (newTokens != null)
                                {
                                    // Update the session with the new tokens
                                    var claimsIdentity = new ClaimsIdentity(transformContext.HttpContext.User.Identity);
                                    claimsIdentity.FindFirst(OpenIdConnectParameterNames.AccessToken)
                                        ?.Let(c => claimsIdentity.RemoveClaim(c));
                                    claimsIdentity.FindFirst(OpenIdConnectParameterNames.RefreshToken)
                                        ?.Let(c => claimsIdentity.RemoveClaim(c));
                                    claimsIdentity.AddClaim(new Claim(OpenIdConnectParameterNames.AccessToken, newTokens.AccessToken));
                                    claimsIdentity.AddClaim(new Claim(OpenIdConnectParameterNames.RefreshToken, newTokens.RefreshToken));

                                    // Note: Add more custom claims like bff custom endpoints (bff:logout_url, bff:session_expires_in and bff:session_state)
                                    var sessionId = claimsIdentity.FindFirst("sid")?.Value;
                                    if (!string.IsNullOrEmpty(sessionId))
                                    {
                                        claimsIdentity.FindFirst("bff:logout_url")?.Let(c => claimsIdentity.RemoveClaim(c));
                                        claimsIdentity.AddClaim(new Claim("bff:logout_url", $"/account/logout?sid={sessionId}"));

                                        var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                                        var sessionState = $"{userId ?? string.Empty}.{sessionId}.{Guid.NewGuid()}";

                                        var sessionCreationTime = DateTimeOffset.UtcNow;
                                        var sessionDuration = TimeSpan.FromHours(1);
                                        var sessionExpiresIn = (int)(sessionCreationTime + sessionDuration - DateTimeOffset.UtcNow).TotalSeconds;

                                        // Add session expiration and state claims
                                        claimsIdentity.FindFirst("bff:session_expires_in")?.Let(c => claimsIdentity.RemoveClaim(c));
                                        claimsIdentity.FindFirst("bff:session_state")?.Let(c => claimsIdentity.RemoveClaim(c));
                                        claimsIdentity.AddClaim(new Claim("bff:session_expires_in", sessionExpiresIn.ToString()));
                                        claimsIdentity.AddClaim(new Claim("bff:session_state", sessionState));
                                    }

                                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                                    var authProperties = new AuthenticationProperties { IsPersistent = true, ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(60) };
                                    authProperties.StoreTokens(new[]
                                    {
                                        new AuthenticationToken { Name = OpenIdConnectParameterNames.AccessToken, Value = newTokens.AccessToken },
                                        new AuthenticationToken { Name = OpenIdConnectParameterNames.RefreshToken, Value = newTokens.RefreshToken }
                                    });
                                    var authService = transformContext.HttpContext.RequestServices.GetRequiredService<IAuthenticationService>();
                                    await authService.SignInAsync(transformContext.HttpContext, bffCookieScheme, claimsPrincipal, authProperties);

                                    // Use the new access token
                                    accessToken = newTokens.AccessToken;
                                }
                                else
                                {
                                    var logger = transformContext.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                                    logger.LogWarning("Failed to refresh access token using the provided refresh token.");
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(accessToken))
                        {
                            transformContext.ProxyRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                        }
                        else
                        {
                            // Log warning if token is not found
                            var logger = transformContext.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                            logger.LogWarning("Access token not found in the session for request to {Path}", transformContext.HttpContext.Request.Path);
                        }

                        // Setting the Cookie header
                        var cookies = transformContext.HttpContext.Request.Headers["Cookie"].ToString();
                        if (!string.IsNullOrEmpty(cookies))
                        {
                            transformContext.ProxyRequest.Headers.Add("Cookie", cookies);
                        }
                        else
                        {
                            var logger = transformContext.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                            logger.LogWarning("No cookies found in the request to forward to the proxy for {Path}", transformContext.HttpContext.Request.Path);
                        }
                    });
                });
        }

        public static void Configure(WebApplication app, IHostEnvironment env)
        {
            const string bffCookieScheme = "BFFCookieScheme";
            const string bffOpenIdConnectChallengeScheme = "BFFChallengeScheme";
            var OidcClientId = app.Configuration["Application:OidcClientId"];
            var OidcClientSecret = app.Configuration["Application:OidcClientSecret"];

            app.UseForwardedHeaders(new ForwardedHeadersOptions()
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            if (!env.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseSession();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapGet("/account/login", async context =>
            {
                await context.ChallengeAsync(bffOpenIdConnectChallengeScheme, new AuthenticationProperties
                {
                    RedirectUri = "/"
                });
            });

            app.MapGet("/account/{area:regex(logout|backchannel)}", async context =>
            {
                var httpClientFactory = context.RequestServices.GetRequiredService<IHttpClientFactory>();
                var client = httpClientFactory.CreateClient("IDPClient");

                var discoveryDocumentResponse = await client.GetDiscoveryDocumentAsync();
                if (discoveryDocumentResponse.IsError)
                {
                    throw new Exception(discoveryDocumentResponse.Error);
                }

                var accessToken = await context.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
                if (!string.IsNullOrEmpty(accessToken))
                {
                    var accessTokenRevocationResponse = await client.RevokeTokenAsync(new TokenRevocationRequest
                    {
                        Address = discoveryDocumentResponse.RevocationEndpoint,
                        ClientId = OidcClientId,
                        ClientSecret = OidcClientSecret,
                        Token = accessToken
                    });

                    if (accessTokenRevocationResponse.IsError)
                    {
                        throw new Exception(accessTokenRevocationResponse.Error);
                    }
                }

                var refreshToken = await context.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);
                if (!string.IsNullOrEmpty(refreshToken))
                {
                    var refreshTokenRevocationResponse = await client.RevokeTokenAsync(new TokenRevocationRequest
                    {
                        Address = discoveryDocumentResponse.RevocationEndpoint,
                        ClientId = OidcClientId,
                        ClientSecret = OidcClientSecret,
                        Token = refreshToken
                    });

                    if (refreshTokenRevocationResponse.IsError)
                    {
                        throw new Exception(refreshTokenRevocationResponse.Error);
                    }
                }

                await context.SignOutAsync(bffCookieScheme);

                await context.SignOutAsync(bffOpenIdConnectChallengeScheme, new AuthenticationProperties
                {
                    RedirectUri = "/"
                });
            });

            // TODO: Investigate why return callback url doesn't work!!
            app.MapGet("/signout-callback-oidc", async context =>
            {
                context.Response.Redirect("/");
            });

            app.MapGet("/account/user", async (HttpContext context, bool slide) =>
            {
                if (!context.User.Identity.IsAuthenticated)
                {
                    return Results.Unauthorized();
                }

                var claims = context.User.Claims.Select(c => new { c.Type, c.Value }).ToList();
                var name = context.User.FindFirst("given_name")?.Value;
                // Note: Oidc doesn't contains email scope.
                // var email = context.User.FindFirst("email")?.Value;

                if (slide)
                {
                    // Get the refresh token from the current session
                    var refreshToken = await context.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);

                    // Note: Fallback >> Try to retrieve it directly from claims if not found in AuthenticationProperties
                    if (string.IsNullOrEmpty(refreshToken))
                    {
                        refreshToken = context.User.FindFirst(OpenIdConnectParameterNames.RefreshToken)?.Value;
                    }

                    if (!string.IsNullOrEmpty(refreshToken))
                    {
                        // Use the existing logic to refresh the access token
                        var newTokens = await RefreshAccessTokenAsync(refreshToken, app.Configuration);

                        if (newTokens != null)
                        {
                            // Update the session with the new tokens
                            var claimsIdentity = new ClaimsIdentity(context.User.Identity);
                            claimsIdentity.FindFirst(OpenIdConnectParameterNames.AccessToken)
                                ?.Then(c => claimsIdentity.RemoveClaim(c));
                            claimsIdentity.FindFirst(OpenIdConnectParameterNames.RefreshToken)
                                ?.Then(c => claimsIdentity.RemoveClaim(c));
                            claimsIdentity.AddClaim(new Claim(OpenIdConnectParameterNames.AccessToken, newTokens.AccessToken));
                            claimsIdentity.AddClaim(new Claim(OpenIdConnectParameterNames.RefreshToken, newTokens.RefreshToken));

                            // Note: Add more custom claims like bff custom endpoints (bff:logout_url, bff:session_expires_in and bff:session_state)
                            var sessionId = claimsIdentity.FindFirst("sid")?.Value;
                            if (!string.IsNullOrEmpty(sessionId))
                            {
                                claimsIdentity.FindFirst("bff:logout_url")?.Let(c => claimsIdentity.RemoveClaim(c));
                                claimsIdentity.AddClaim(new Claim("bff:logout_url", $"/account/logout?sid={sessionId}"));

                                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                                var sessionState = $"{userId ?? string.Empty}.{sessionId}.{Guid.NewGuid()}";

                                var sessionCreationTime = DateTimeOffset.UtcNow;
                                var sessionDuration = TimeSpan.FromHours(1);
                                var sessionExpiresIn = (int)(sessionCreationTime + sessionDuration - DateTimeOffset.UtcNow).TotalSeconds;

                                // Add session expiration and state claims
                                claimsIdentity.FindFirst("bff:session_expires_in")?.Let(c => claimsIdentity.RemoveClaim(c));
                                claimsIdentity.FindFirst("bff:session_state")?.Let(c => claimsIdentity.RemoveClaim(c));
                                claimsIdentity.AddClaim(new Claim("bff:session_expires_in", sessionExpiresIn.ToString()));
                                claimsIdentity.AddClaim(new Claim("bff:session_state", sessionState));
                            }

                            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                            var authProperties = new AuthenticationProperties();
                            authProperties.StoreTokens(new[]
                            {
                                new AuthenticationToken { Name = OpenIdConnectParameterNames.AccessToken, Value = newTokens.AccessToken },
                                new AuthenticationToken { Name = OpenIdConnectParameterNames.RefreshToken, Value = newTokens.RefreshToken }
                            });

                            await context.SignInAsync(bffCookieScheme, claimsPrincipal, authProperties);
                        }
                        else
                        {
                            return Results.Unauthorized(); // Failed to refresh the token
                        }
                    }
                    else
                    {
                        return Results.Unauthorized(); // No refresh token found
                    }
                }

                //return Results.Ok(new { Name = name, Claims = claims });
                return Results.Ok(claims);
            });

            app.MapReverseProxy(); // Map YARP reverse proxy
        }

        private static async Task<TokenResponse> RefreshAccessTokenAsync(string refreshToken, IConfiguration configuration)
        {
            var client = new HttpClient();
            var tokenEndpoint = $"{configuration["Application:IdPAuthority"]}/connect/token";
            var clientId = configuration["Application:OidcClientId"];
            var clientSecret = configuration["Application:OidcClientSecret"];

            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { OpenIdConnectParameterNames.GrantType, OpenIdConnectParameterNames.RefreshToken },
                { OpenIdConnectParameterNames.RefreshToken, refreshToken },
                { OpenIdConnectParameterNames.ClientId, clientId! },
                { OpenIdConnectParameterNames.ClientSecret, clientSecret! }
            });

            var tokenResponse = await client.PostAsync(tokenEndpoint, content);

            if (tokenResponse.IsSuccessStatusCode)
            {
                return await tokenResponse.Content.ReadFromJsonAsync<TokenResponse>();
            }

            return null;
        }

        private static async Task<bool> TokenIsExpired(string accessToken, IConfiguration configuration)
        {
            // Note: Token type is: AccessTokenType.Reference
            var introspectionEndpoint = $"{configuration["Application:IdPAuthority"]}/connect/introspect";
            var clientId = configuration["Application:OidcClientId"];
            var clientSecret = configuration["Application:OidcClientSecret"];

            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, introspectionEndpoint);
            
            request.Content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("token", accessToken), // Note: introspected works with token instead access_token
                new KeyValuePair<string, string>(OpenIdConnectParameterNames.ClientId, clientId),
                new KeyValuePair<string, string>(OpenIdConnectParameterNames.ClientSecret, clientSecret)
            });

            try
            {
                // Note: Send the request to the introspection endpoint
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var tokenData = JsonSerializer.Deserialize<Dictionary<string, object>>(content);

                if (tokenData.ContainsKey("active") && (bool)tokenData["active"])
                {
                    var expiryDate = DateTimeOffset.FromUnixTimeSeconds((long)tokenData["exp"]);
                    return expiryDate < DateTime.UtcNow;
                }

                // Note: If the token is not active, consider it expired
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred while introspecting the token: {ex.Message}");
                // Note: If there's an error during introspection, treat the token as expired
                return true;
            }
        }
    }
    
    public class TokenResponse
    {
        [JsonPropertyName(OpenIdConnectParameterNames.AccessToken)]
        public string AccessToken { get; set; }

        [JsonPropertyName(OpenIdConnectParameterNames.ExpiresIn)]
        public int ExpiresIn { get; set; }

        [JsonPropertyName(OpenIdConnectParameterNames.RefreshToken)]
        public string RefreshToken { get; set; }

        [JsonPropertyName(OpenIdConnectParameterNames.TokenType)]
        public string TokenType { get; set; }

        [JsonPropertyName(OpenIdConnectParameterNames.Scope)]
        public string Scope { get; set; }
    }

    public static class FunctionalExtensions
    {
        public static void Let<T>(this T obj, Action<T> action) where T : class
        {
            if (obj != null)
            {
                action(obj);
            }
        }

        public static TResult Then<TSource, TResult>(this TSource source, Func<TSource, TResult> func) where TSource : class
        {
            return source != null ? func(source) : default;
        }

        public static void Then<TSource>(this TSource source, Action<TSource> action) where TSource : class
        {
            if (source != null)
            {
                action(source);
            }
        }
    }
}
