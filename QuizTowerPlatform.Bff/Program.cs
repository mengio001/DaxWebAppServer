using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Yarp.ReverseProxy.Transforms;

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
            var apiRoot = configuration["Application:BackendBaseAddress"];
            var idpAuthority = configuration["Application:IdPAuthority"];
            var idPAudience = configuration["Application:IdPAudience"];

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
                options.AccessDeniedPath = "/Authentication/AccessDenied";
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
                options.ResponseMode = "query";
                options.SaveTokens = true;
                // Specify where the user will be redirected after a successful login
                options.CallbackPath = "/signin-oidc";
                // Specify where the user will be redirected after logout || "/signout-callback-oidc" || "/signout-oidc"
                options.SignedOutCallbackPath = "/signout-callback-oidc";
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
                    NameClaimType = "given_name",
                    RoleClaimType = "role",
                };
            });

            services.AddAuthorization();

            // YARP reverse proxy services - src: https://microsoft.github.io/reverse-proxy/articles/getting-started.html
            services.AddReverseProxy()
                .LoadFromConfig(Configuration.GetSection("ReverseProxy"))
                .AddTransforms(builderContext =>
                {
                    builderContext.AddRequestTransform(async transformContext =>
                    {
                        var accessToken = await transformContext.HttpContext.GetTokenAsync("access_token");
                        if (!string.IsNullOrEmpty(accessToken))
                        {
                            transformContext.ProxyRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                        }
                    });
                });
        }

        public static void Configure(WebApplication app, IHostEnvironment env)
        {
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

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapReverseProxy(); // Map YARP reverse proxy
        }
    }
}
