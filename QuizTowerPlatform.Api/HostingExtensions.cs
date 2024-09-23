using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using QuizTowerPlatform.Api.Configurations;
using Microsoft.AspNetCore.HttpOverrides;
using QuizTowerPlatform.Api.Services.Implementations;
using QuizTowerPlatform.Api.Services.Interfaces;
using QuizTowerPlatform.Api.Util;
using Serilog;

namespace QuizTowerPlatform.Api
{
    internal static class HostingExtensions
    {
        public static WebApplication ConfigureServices(this WebApplicationBuilder builder, IConfiguration configuration)
        {
            builder.Services.AddApplicationInsightsTelemetry();

            builder.Services.AddCors(options =>
            {
                // Adjust the following to match the specifications of the consuming application!
                options.AddDefaultPolicy(builder => builder.WithOrigins(configuration.GetSection("CORS_Url").Value!).AllowAnyMethod().AllowAnyHeader().AllowCredentials());
            });

            builder.Services.AddDbContext(builder.Configuration);
            builder.Services.AddApiClients(builder.Configuration);
            builder.Services.AddApiServices(builder.Configuration);
            builder.Services.AddControllersWithSecurityFilters()
            .AddJsonOptions(configure => configure.JsonSerializerOptions.PropertyNamingPolicy = null);

            builder.Services.AddScoped<IQuizService, QuizService>();
            builder.Services.AddScoped<IQuestionService, QuestionService>();
            builder.Services.AddScoped<IUserResultService, UserResultService>();
            builder.Services.AddScoped<IUserInfoService, UserInfoService>();
            builder.Services.AddScoped<IAchievementService, AchievementService>();

            builder.Services.AddMemoryCache();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddOAuth2Introspection(options =>
            {
                options.Authority = builder.Configuration["Application:IdPAuthority"]; // Note: middleware IDP entry-point URI
                options.ClientId = builder.Configuration["Application:IdPAudience"];
                options.ClientSecret = builder.Configuration["Application:IdPAudienceSecret"];
                options.NameClaimType = "given_name";
                options.RoleClaimType = "role";
            });

            builder.Services.AddAuthorization(authorizationOptions =>
            {
                var idPAudience = configuration.GetValue<string>("Application:IdPAudience")!;

                authorizationOptions.AddPolicy("ClientApplicationCanRead", policyBuilder => 
                {
                    policyBuilder.RequireClaim("scope", $"{idPAudience}.read");
                });

                authorizationOptions.AddPolicy("ClientApplicationCanWrite", policyBuilder => 
                {
                    policyBuilder.RequireClaim("scope", $"{idPAudience}.write");
                });

                authorizationOptions.AddPolicy("UserCanAddQuiz", policyBuilder => 
                {
                    policyBuilder.RequireAuthenticatedUser();
                    policyBuilder.RequireRole(["SecurityAdmin", "QuizMaster"]);
                });

                authorizationOptions.AddPolicy("UserCanEditQuiz", policyBuilder => 
                {
                    policyBuilder.RequireAuthenticatedUser();
                    policyBuilder.RequireRole(["SecurityAdmin", "QuizMaster"]);
                });
            });

            if (configuration.GetValue<bool>("EnableSwagger"))
            {
                builder.Services.AddSwaggerDocumentation(configuration.GetValue<string>("Application:IdPAuthority")!, configuration.GetValue<string>("Application:DisplayName")!);
            }

            return builder.Build();
        }

        public static WebApplication ConfigurePipeline(this WebApplication app)
        {
            if (app.Environment.IsDevelopment() || app.Environment.IsTest())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseForwardedHeaders(new ForwardedHeadersOptions()
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseSerilogRequestLogging();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseCors();

            if (app.Configuration.GetValue<bool>("EnableSwagger"))
            {
                app.UseSwaggerDocumentation(app.Configuration.GetValue<string>("Application:Name")!, app.Configuration.GetValue<string>("Application:IdPAudience")!);
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            return app;
        }
    }
}
