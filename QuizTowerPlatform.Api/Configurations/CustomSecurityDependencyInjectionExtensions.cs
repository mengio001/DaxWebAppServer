using Microsoft.AspNetCore.Mvc;
using QuizTowerPlatform.Api.Filters;

namespace QuizTowerPlatform.Api.Configurations
{
    public static class CustomSecurityDependencyInjectionExtensions
    {

        /// <summary>
        /// Add controllers with all necessary filters that handle security.
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="configure">Optional custom configuration adjustments</param>
        /// <returns></returns>
        public static IMvcBuilder AddControllersWithSecurityFilters(this IServiceCollection services, Action<MvcOptions>? configure = null)
        {
            services.AddAntiforgery(options =>
            {
                options.HeaderName = "X-CSRF-TOKEN";
            });
            return services.AddControllers(options =>
            {
                options.Filters.Add(typeof(RequestAntiforgeryFilter));
                configure?.Invoke(options);
            });
        }
    }
}
