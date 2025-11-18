using  Microsoft.AspNetCore.Http;
using  Microsoft.Extensions.Logging;
using  System;
using  System.Net;
using  System.Text.Json;
using  System.Threading.Tasks;

namespace AutoBarato.Comunicaciones.Api.Middleware
{
    // Captura errores globalmente sin necesidad de usar try-catch en cada controlador.
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error no manejado en la API.");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var response = new
            {
                StatusCode = (int)HttpStatusCode.InternalServerError,
                Message = "Ocurrió un error en el servidor.",
                Details = exception.Message
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
