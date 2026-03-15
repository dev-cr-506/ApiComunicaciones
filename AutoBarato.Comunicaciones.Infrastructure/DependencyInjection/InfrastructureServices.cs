using AutoBarato.Comunicaciones.Application.Interfaces;
using AutoBarato.Comunicaciones.Application.Interfaces.Bitacora;
using AutoBarato.Comunicaciones.Application.Interfaces.Contexto;
using AutoBarato.Comunicaciones.Domain.Interfaces.Bitacora;
using AutoBarato.Comunicaciones.Domain.Interfaces.Chat;
using AutoBarato.Comunicaciones.Infrastructure.Bitacora.Error;
using AutoBarato.Comunicaciones.Infrastructure.Bitacora.Evento;
using AutoBarato.Comunicaciones.Infrastructure.Contexto;
using AutoBarato.Comunicaciones.Infrastructure.DataAccess;
using AutoBarato.Comunicaciones.Infrastructure.DataAccess.Repositories;
using AutoBarato.Comunicaciones.Infrastructure.Services;
using AutoBarato.Comunicaciones.Infrastructure.DataAccess.Repositories.Bitacora;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;


namespace AutoBarato.Comunicaciones.Infrastructure.DependencyInjection
{
    public static class InfrastructureServices
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection servicios, IConfiguration configuracion)
        {

            var connectionString = configuracion.GetConnectionString("ConexionSql");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new Exception("⚠️ Error: No se encontró la cadena de conexión en appsettings.json.");
            }

            servicios.AddDbContext<ComunicacionesDbContext>(opciones => opciones.UseSqlServer(connectionString));



            ConfiguracionDeHttpClient.AddHttpClients(servicios, configuracion);
            servicios.AddScoped<IChatRepository, ChatRepository>();
            servicios.AddScoped<IServicioDeTransacciones, ServicioDeTransacciones>();
            servicios.AddScoped<EjecutorDeProcedimientosAlmacenados>();

            servicios.AddHttpClient();
            servicios.AddScoped<IApiGatewayService, ApiGatewayService>();
            servicios.AddScoped<IContextoDeEjecucion, ContextoDeEjecucionHttp>();

            servicios.AddScoped<IServicioDeTransacciones, ServicioDeTransacciones>();
            servicios.AddScoped<EjecutorDeProcedimientosAlmacenados>();



            servicios.Configure<BitacoraErrorOptions>(configuracion.GetSection("Bitacora"));

            servicios.AddSingleton<BitacoraColaDeErrores>(sp =>
            {
                var opt = sp.GetRequiredService<IOptions<BitacoraErrorOptions>>().Value;
                return new BitacoraColaDeErrores(opt.QueueCapacity);
            });

            servicios.AddSingleton<IBitacoraErrorService, BitacoraErrorService>();
            servicios.AddScoped<IBitacoraDeErrorRepository, BitacoraDeErrorRepository>();
            servicios.AddHostedService<BitacoraErroresBackgroundService>();

            servicios.Configure<BitacoraEventosOptions>(configuracion.GetSection("BitacoraEventos"));

            servicios.AddSingleton<BitacoraColaDeEventos>(sp =>
            {
                var opt = sp.GetRequiredService<IOptions<BitacoraEventosOptions>>().Value;
                return new BitacoraColaDeEventos(opt.QueueCapacity);
            });

            servicios.AddSingleton<IBitacoraEventosService, BitacoraEventosService>();
            servicios.AddScoped<IBitacoraEventoRepository, BitacoraEventoRepository>();
            servicios.AddHostedService<BitacoraEventosBackgroundService>();

            return servicios;
        }
    }
}
