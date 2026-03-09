using AutoBarato.Comunicaciones.Application.Interfaces;
using AutoBarato.Comunicaciones.Application.Interfaces.Bitacora;
using AutoBarato.Comunicaciones.Application.Interfaces.Contexto;
using AutoBarato.Comunicaciones.Application.Services;
using AutoBarato.Comunicaciones.Domain.Interfaces.Bitacora;
using AutoBarato.Comunicaciones.Domain.Interfaces.Chat;
using AutoBarato.Comunicaciones.Infrastructure.Bitacora.Error;
using AutoBarato.Comunicaciones.Infrastructure.Bitacora.Evento;
using AutoBarato.Comunicaciones.Infrastructure.Contexto;
using AutoBarato.Comunicaciones.Infrastructure.DataAccess;
using AutoBarato.Comunicaciones.Infrastructure.DataAccess.Repositories;
using AutoBarato.Comunicaciones.Infrastructure.Services;
using AutoBarato.ServiciosAutomotrices.Infrastructure.Bitacora.Evento;
using AutoBarato.ServiciosAutomotrices.Infrastructure.DataAccess.Repositories.Bitacora;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoBarato.Comunicaciones.Infrastructure.DependencyInjection
{
    public static class InfrastructureServices
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {

            var connectionString = configuration.GetConnectionString("ConexionSql");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new Exception("⚠️ Error: No se encontró la cadena de conexión en appsettings.json.");
            }

            services.AddDbContext<ComunicacionesDbContext>(options => options.UseSqlServer(connectionString));


            // 🔹 Registrar configuraciones desde appsettings.json
            //services.Configure<ExternalServicesConfig>(configuration.GetSection("ExternalServices"));

            // 🔹 Configurar HttpClients
            ConfiguracionDeHttpClient.AddHttpClients(services, configuration);
            services.AddScoped<IChatRepository, ChatRepository>();
            services.AddScoped<IServicioDeTransacciones, ServicioDeTransacciones>();
            services.AddScoped<EjecutorDeProcedimientosAlmacenados>();

            services.AddHttpClient();
            services.AddScoped<IApiGatewayService, ApiGatewayService>();
            services.AddScoped<IContextoDeEjecucion, ContextoDeEjecucionHttp>();

            services.AddScoped<IServicioDeTransacciones, ServicioDeTransacciones>();
            services.AddScoped<EjecutorDeProcedimientosAlmacenados>();



            services.Configure<BitacoraErrorOptions>(configuration.GetSection("Bitacora"));

            services.AddSingleton<BitacoraColaDeErrores>(sp =>
            {
                var opt = sp.GetRequiredService<IOptions<BitacoraErrorOptions>>().Value;
                return new BitacoraColaDeErrores(opt.QueueCapacity);
            });

            services.AddSingleton<IBitacoraErrorService, BitacoraErrorService>();
            services.AddScoped<IBitacoraDeErrorRepository, BitacoraDeErrorRepository>();
            services.AddHostedService<BitacoraErroresBackgroundService>();

            services.Configure<BitacoraEventosOptions>(configuration.GetSection("BitacoraEventos"));

            services.AddSingleton<BitacoraColaDeEventos>(sp =>
            {
                var opt = sp.GetRequiredService<IOptions<BitacoraEventosOptions>>().Value;
                return new BitacoraColaDeEventos(opt.QueueCapacity);
            });

            services.AddSingleton<IBitacoraEventosService, BitacoraEventosService>();
            services.AddScoped<IBitacoraEventoRepository, BitacoraEventoRepository>();
            services.AddHostedService<BitacoraEventosBackgroundService>();

            return services;
        }
    }
}
