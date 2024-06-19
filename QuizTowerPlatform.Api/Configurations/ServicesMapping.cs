using Microsoft.EntityFrameworkCore;
using QuizTowerPlatform.Api.Accessors;
using QuizTowerPlatform.Api.Services;
using QuizTowerPlatform.Data.Context;

namespace QuizTowerPlatform.Api.Configurations
{
    public static class ServicesMapping
    {
        public static void AddDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<IApiDbContext, ApiDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        }

        public static void AddApiServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IRequestAccessor, RequestAccessor>();
            services.AddSingleton<Services.Security.RelationCheck.IAuthenticationService, Services.Security.RelationCheck.AuthenticationService>();
        }

        public static void AddApiClients(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient<Services.ITokenService, Services.TokenService>((sp, client) =>
            {
                client.BaseAddress = new Uri(configuration.GetValue<string>("Application:IdPAuthority"));
            });
        }
    }
}
