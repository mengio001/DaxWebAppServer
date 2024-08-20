using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace QuizTowerPlatform.Api.Configurations
{
    public static class SwaggerExtensions
    {
        public static void AddSwaggerDocumentation(this IServiceCollection services, string authority, string docDisplayName)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = $"{docDisplayName}",
                });
                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        AuthorizationCode = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri($"{authority}/connect/authorize"),
                            TokenUrl = new Uri($"{authority}/connect/token"),
                            Scopes = new Dictionary<string, string>
                            {
                                { "openid", "openid" },
                                { "offline_access", "offline_access" },
                                { "profile", "profile" },
                                { "roles", "roles" },
                                { "country", "country" },
                                { "towerofquizzesapi.fullaccess", "towerofquizzesapi.fullaccess" },
                                { "towerofquizzesbffapi.fullaccess", "towerofquizzesbffapi.fullaccess" },
                                { "usermanagementapi.fullaccess", "usermanagementapi.fullaccess"}
                            }
                        },
                        ClientCredentials = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri($"{authority}/connect/authorize"),
                            TokenUrl = new Uri($"{authority}/connect/token"),
                            Scopes = new Dictionary<string, string>
                            {
                                {"anonimiseren", "anonimiseren"},
                            }
                        }
                    }
                });

                options.OperationFilter<AddCrossSiteRequestForgeryTokenHeaderParameter>();
                options.OperationFilter<AuthorizeCheckOperationFilter>();
            });
        }

        public static void UseSwaggerDocumentation(this IApplicationBuilder app, string appTitle, string idPSwaggerAudience)
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", $"{appTitle}");
                options.OAuthClientId($"{idPSwaggerAudience}_swagger"); //Client id moet bekend zijn bij identityserver
                options.OAuthAppName($"{appTitle} - Swagger");
                options.OAuthUsePkce();
            });
        }
    }

    // Note: CSRF (Cross-Site Request Forgery) tokens are used to prevent unauthorized commands from being transmitted from a user that the web application trusts.
    // When a user is authenticated, a CSRF token is typically included in the form of a hidden field in forms, or as a custom header in AJAX requests,
    // to ensure that the request is coming from the authenticated user and not a malicious third party.
    public class AddCrossSiteRequestForgeryTokenHeaderParameter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            operation.Parameters ??= new List<OpenApiParameter>();

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "X-CSRF-TOKEN",
                In = ParameterLocation.Header,
                Description = "Http header 'X-CSRF-TOKEN'. Tip: Kopieer de cookie waarde van 'CSRF-TOKEN' cookie (verplicht bij PUT, POST en DELETE)",
                Required = false
            });
        }
    }

    public class AuthorizeCheckOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var hasAuthorize = context.MethodInfo.DeclaringType!.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any() ||
                               context.MethodInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any();

            if (hasAuthorize)
            {
                operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
                operation.Responses.Add("403", new OpenApiResponse { Description = "Forbidden" });
                operation.Security = new List<OpenApiSecurityRequirement>
                {
                    new OpenApiSecurityRequirement
                    {
                        [
                            new OpenApiSecurityScheme {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "oauth2"
                                }
                            }
                        ] = new[] { "towerofquizzesapi" }
                    }
                };
            }
        }
    }
}
