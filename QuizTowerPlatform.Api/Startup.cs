using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc;
using QuizTowerPlatform.Api.Configurations;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.HttpOverrides;
using Serilog;

[assembly: ApiController]
[assembly: ApiConventionType(typeof(ApiConventions))]
namespace QuizTowerPlatform.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        private IWebHostEnvironment Environment { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        // This method is triggered by the runtime. Utilize this method to configure and register services within the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationInsightsTelemetry();

            services.AddCors(options =>
            {
                // Adjust the following to match the specifications of the consuming application!
                options.AddDefaultPolicy(builder => builder.WithOrigins(Configuration.GetSection("CORS_Url").Value!).AllowAnyMethod().AllowAnyHeader().AllowCredentials());
            });

            services.AddControllersWithSecurityFilters();

            //https://github.com/AzureAD/azure-activedirectory-identitymodel-extensions-for-dotnet/issues/415#issuecomment-759871786
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                //.AddJwtBearer(options =>
                //{
                //    options.Authority = Configuration.GetValue<string>("Application:IdPAuthority");
                //    options.TokenValidationParameters.ValidateAudience = false;
                //    options.TokenValidationParameters.ValidTypes = new[] { "at+jwt" };
                //});
                .AddJwtBearer(options =>
                {
                    options.Authority = Configuration.GetValue<string>("Application:IdPAuthority");
                    options.Audience = Configuration.GetValue<string>("Application:IdPAudience");
                    options.TokenValidationParameters = new()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        NameClaimType = "given_name",
                        RoleClaimType = "role",
                        ValidTypes = new[] { "at+jwt" } // Note: no more needed by AddOAuth2Introspection to prevent JWT Token attack! because there is nothing to decode and read with reference token.
                    };
                });

            services.AddMemoryCache();
            services.AddHttpContextAccessor();

            if (Configuration.GetValue<bool>("EnableSwagger"))
            {
                services.AddSwaggerDocumentation(Configuration.GetValue<string>("Application:IdPAuthority")!, Configuration.GetValue<string>("Application:DisplayName")!);
            }
            services.AddDbContext(Configuration);
            services.AddApiClients(Configuration);
            services.AddApiServices(Configuration);

            services.AddAuthorization(authorizationOptions =>
            {
                var idPAudience = Configuration.GetValue<string>("Application:IdPAudience")!;

                authorizationOptions.AddPolicy("ClientApplicationCanRead", policyBuilder =>
                {
                    policyBuilder.RequireClaim("scope", $"{idPAudience}.read");
                });

                authorizationOptions.AddPolicy("ClientApplicationCanWrite", policyBuilder =>
                {
                    policyBuilder.RequireClaim("scope", $"{idPAudience}.write");
                });

                authorizationOptions.AddPolicy("MustOwnQuiz", policyBuilder =>
                {
                    policyBuilder.RequireAuthenticatedUser();
                    // policyBuilder.AddRequirements(new MustOwnQuizRequirement());
                });

                authorizationOptions.AddPolicy("ApiCaller", policy =>
                {
                    policy.RequireClaim("scope", "towerofquizzesapi.read", "towerofquizzesapi.write");
                });

                authorizationOptions.AddPolicy("InteractiveUser", policy =>
                {
                    policy.RequireClaim("sub");
                });
            });

            services.Configure<JsonOptions>(options =>
            {
                // Hide fields with null values.
                options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
                options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
            });
        }

        // This method is invoked by the runtime. Use this method to set up the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseForwardedHeaders(new ForwardedHeadersOptions()
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseSerilogRequestLogging();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            if (Configuration.GetValue<bool>("EnableSwagger"))
            {
                app.UseSwaggerDocumentation(Configuration.GetValue<string>("Application:Name")!, Configuration.GetValue<string>("Application:IdPAudience")!);
            }

        }
    }
}