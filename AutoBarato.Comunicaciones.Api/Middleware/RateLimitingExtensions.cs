using  Microsoft.Extensions.DependencyInjection;

namespace AutoBarato.Comunicaciones.Api.Middleware
{
    public static class RateLimitingExtensions
    {
        public static IServiceCollection AddCustomRateLimiting(
            this IServiceCollection services,
            Action<RateLimitingOptions> configure)
        {
            var opts = new RateLimitingOptions();
            configure?.Invoke(opts);
            services.AddSingleton(opts);
            services.AddMemoryCache();
            return services;
        }
    }
}
