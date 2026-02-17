using  Microsoft.Extensions.DependencyInjection;
using  System.Reflection;
using AutoBarato.Comunicaciones.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using AutoBarato.Comunicaciones.Application.Services;

namespace AutoBarato.Comunicaciones.Application.DependencyInjection
{
    public static class ApplicationServices
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Registrar AutoMapper

            // Registrar servicios de repositorio
            services.AddScoped<IChatService, ChatService>();
            services.AddScoped<IChatMediaService, ChatMediaService>();
            services.AddScoped<IApiFilesService, ApiFilesService>();

            return services;
        }
    }

}
