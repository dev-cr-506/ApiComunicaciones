using Microsoft.Extensions.DependencyInjection;
using AutoBarato.Comunicaciones.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using AutoBarato.Comunicaciones.Application.Services.Chat;

namespace AutoBarato.Comunicaciones.Application.DependencyInjection
{
    public static class ServiciosDeAplicacion
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection servicios, IConfiguration configuracion)
        {
            servicios.AddScoped<IChatService, ChatService>();
            servicios.AddScoped<IChatMediaService, ChatMediaService>();

            return servicios;
        }
    }

}
