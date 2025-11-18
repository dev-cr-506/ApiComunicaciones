using AutoBarato.Comunicaciones.Application.Interfaces;
using AutoBarato.Comunicaciones.Application.Services;
using AutoBarato.Comunicaciones.Domain.Interfaces;
using AutoBarato.Comunicaciones.Infrastructure.DataAccess;
using AutoBarato.Comunicaciones.Infrastructure.DataAccess.Repositories;
using AutoBarato.Comunicaciones.Infrastructure.Services;
using  Microsoft.Extensions.Configuration;
using  Microsoft.Extensions.DependencyInjection;
using  System;
using  System.Collections.Generic;
using  System.Linq;
using  System.Text;
using  System.Threading.Tasks;

namespace AutoBarato.Comunicaciones.Infrastructure.DependencyInjection
{
    public static class InfrastructureServices
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // 🔹 Registrar configuraciones desde appsettings.json
            //services.Configure<ExternalServicesConfig>(configuration.GetSection("ExternalServices"));

            // 🔹 Configurar HttpClients
            HttpClientConfiguration.AddHttpClients(services, configuration);
            services.AddScoped<IChatRepository, ChatRepository>();
            services.AddScoped<ITransactionalService, TransactionalService>();
            services.AddScoped<StoredProcedureExecutor>();

            return services;
        }
    }
}
