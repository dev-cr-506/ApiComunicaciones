using Microsoft.Extensions.DependencyInjection;
using AutoBarato.Comunicaciones.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using AutoBarato.Comunicaciones.Application.Services.Chat;

namespace AutoBarato.Comunicaciones.Application.DependencyInjection
{
    public static class ServiciosDeAplicacion
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Registrar AutoMapper

            // Registrar servicios de repositorio
            services.AddScoped<IChatService, ChatService>();
            services.AddScoped<IChatMediaService, ChatMediaService>();

            return services;
        }
    }

}
