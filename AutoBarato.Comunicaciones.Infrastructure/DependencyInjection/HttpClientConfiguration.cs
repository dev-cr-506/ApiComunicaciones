using  Microsoft.Extensions.Configuration;
using  Microsoft.Extensions.DependencyInjection;
using  System;
using  System.Collections.Generic;
using  System.Linq;
using  System.Text;
using  System.Threading.Tasks;

namespace AutoBarato.Comunicaciones.Infrastructure.DependencyInjection
{
    public static class HttpClientConfiguration
    {
        public static IServiceCollection AddHttpClients(this IServiceCollection services, IConfiguration configuration)
        {
            return services;
        }
    }
}
