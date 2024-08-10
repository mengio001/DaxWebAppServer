using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.HttpOverrides;

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

            var apiRoot = configuration["Application:BackendBaseAddress"];
            var idpAuthority = configuration["Application:IdPAuthority"];
            var idPAudience = configuration["Application:IdPAudience"];

            // Add authentication and authorization services
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.AccessDeniedPath = "/Authentication/AccessDenied";
            })
            .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
            {
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.Authority = $"{idpAuthority}";
                options.ClientId = configuration["Application:OidcClientId"];
                options.ClientSecret = configuration["Application:OidcClientSecret"];
                options.ResponseType = "code";
                options.SaveTokens = true;
                options.GetClaimsFromUserInfoEndpoint = true;
                options.Scope.Clear();
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.Scope.Add("towerofquizzesapi.read");
                options.Scope.Add("towerofquizzesapi.write");
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
                .LoadFromConfig(Configuration.GetSection("ReverseProxy"));
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
