using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc;
using QuizTowerPlatform.Api.Configurations;
using System.IdentityModel.Tokens.Jwt;
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

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationInsightsTelemetry();

            services.AddCors(options =>
            {
                // Onderstaande aanpassen naar de specificatie van de consumerende applicatie!
                options.AddDefaultPolicy(builder => builder.WithOrigins(Configuration.GetSection("CORS_Url").Value).AllowAnyMethod().AllowAnyHeader().AllowCredentials());
            });

            //https://github.com/AzureAD/azure-activedirectory-identitymodel-extensions-for-dotnet/issues/415#issuecomment-759871786
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
                        NameClaimType = "given_name",
                        RoleClaimType = "role",
                        ValidTypes = new[] { "at+jwt" } // note: no more needed by AddOAuth2Introspection to prevent JWT Token attack! because there is nothing to decode and read with reference token.
                    };
                });

            services.AddMemoryCache();
            //services.AddFluentValidationConfiguration();
            services.AddHttpContextAccessor();
            if (Configuration.GetValue<bool>("EnableSwagger"))
            {
                services.AddSwaggerDocumentation(Configuration.GetValue<string>("Application:IdPAuthority"), Configuration.GetValue<string>("Application:DisplayName"));
            }
            services.AddDbContext(Configuration);
            services.AddApiClients(Configuration);
            services.AddApiServices(Configuration);
            //services.AddExceptionAndLogHandlingServices(Environment);
            services.AddAuthorization();
            services.Configure<JsonOptions>(options =>
            {
                // Hide fields with null values.
                options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
                options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseForwardedHeaders(new ForwardedHeadersOptions()
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            //app.UseCustomExceptionHandling();
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
                app.UseSwaggerDocumentation(Configuration.GetValue<string>("Application:Name"), Configuration.GetValue<string>("Application:IdPAudience"));
            }

        }
    }
}