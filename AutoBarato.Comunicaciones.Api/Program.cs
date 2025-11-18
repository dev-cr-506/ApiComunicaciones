using System.Reflection;
using AutoBarato.Comunicaciones.Api;
using AutoBarato.Comunicaciones.Application.DependencyInjection;
using AutoBarato.Comunicaciones.Infrastructure.DataAccess;
using AutoBarato.Comunicaciones.Infrastructure.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// 🔹 Configuración de servicios
ConfigureServices(builder);

var app = builder.Build();

// 🔹 Configuración del pipeline/middleware
ConfigureMiddleware(app);

app.Run();


// ======================= Métodos de configuración =======================

void ConfigureServices(WebApplicationBuilder builder)
{
    // 🔐 Configuración de la base de datos
    ConfigureDatabase(builder);

    // 📁 Configuración de capas (Infraestructura + Aplicación)
    builder.Services.AddInfrastructure(builder.Configuration);
    builder.Services.AddApplicationServices(builder.Configuration);

    // 🧠 Helpers y utilidades
    builder.Services.AddHttpClient();
    builder.Services.AddMemoryCache();
    builder.Services.AddSignalR();

    // 🎨 AutoMapper con todos los perfiles del proyecto
    builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

    // 📡 Servicios API (controllers, swagger, compresión)
    ConfigureApiServices(builder.Services);

    // 🌐 CORS
    ConfigureCors(builder.Services);
}

void ConfigureDatabase(WebApplicationBuilder builder)
{
    var connectionString = builder.Configuration.GetConnectionString("ConexionSql");

    if (string.IsNullOrWhiteSpace(connectionString))
        throw new Exception("⚠️ Error: No se encontró la cadena de conexión 'ConexionSql' en appsettings.json.");

    builder.Services.AddDbContext<ComunicacionesDbContext>(options =>
        options.UseSqlServer(connectionString));
}

void ConfigureApiServices(IServiceCollection services)
{
    services.AddControllers();
    services.AddEndpointsApiExplorer();

    services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "API Comunicaciones - AutoBarato",
            Version = "v1",
            Description = "Módulo encargado de la comunicación en AutoBarato."
        });

        // Incluir comentarios XML si el archivo existe
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath))
        {
            options.IncludeXmlComments(xmlPath);
        }
    });

    services.AddResponseCompression(options => options.EnableForHttps = true);
}

//void ConfigureCors(IServiceCollection services)
//{
//    services.AddCors(options =>
//    {
//        // CORS general para el frontend
//        options.AddPolicy("AllowFrontendAutoBarato", policy =>
//        {
//            policy.WithOrigins(
//                    "https://frontend.autobarato.com",
//                    "http://localhost:5173"
//                )
//                .AllowAnyHeader()
//                .AllowAnyMethod();
//        });

//        // CORS específico para SignalR si lo necesitas separado
//        options.AddPolicy("SignalRCors", policy =>
//        {
//            policy.WithOrigins(
//                    "http://localhost:5173",
//                    "https://tusitio.autobarato.cr"
//                )
//                .AllowAnyHeader()
//                .AllowAnyMethod()
//                .AllowCredentials();
//        });
//    });
//}

void ConfigureCors(IServiceCollection services)
{
    services.AddCors(options =>
    {
        options.AddPolicy("AllowFrontendAutoBarato", policy =>
        {
            policy
                .WithOrigins("https://frontend.autobarato.com", "http://localhost:5173")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });

        options.AddPolicy("SignalRCors", policy =>
        {
            policy
                .WithOrigins("http://localhost:5173", "https://tusitio.autobarato.cr")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
    });
}

void ConfigureMiddleware(WebApplication app)
{
    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }

    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Comunicaciones - AutoBarato v1");
        c.DocumentTitle = "SW - API Comunicaciones";
    });

    app.UseHttpsRedirection();

    // Política general para controllers
    app.UseCors("AllowFrontendAutoBarato");

    app.UseAuthorization();
    app.UseResponseCompression();

    // Para el hub, aplicar explícitamente la política con credenciales:
    app.MapHub<CarChatHub>("/hubs/car-chat")
       .RequireCors("SignalRCors"); // 👈 clave

    app.MapControllers();
}
