using  Microsoft.Extensions.Configuration;
using  Microsoft.Extensions.DependencyInjection;

namespace AutoBarato.Comunicaciones.Infrastructure.DependencyInjection
{
    public static class ConfiguracionDeHttpClient
    {
        public static IServiceCollection AddHttpClients(this IServiceCollection servicios, IConfiguration configuracion)
        {
            return servicios;
        }
    }
}
