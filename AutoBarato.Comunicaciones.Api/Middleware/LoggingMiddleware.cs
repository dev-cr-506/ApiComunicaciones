using  System.Diagnostics;

namespace AutoBarato.Comunicaciones.Api.Middleware
{
    // Cada petición se logueará con el tiempo de respuesta, ayudando a detectar cuellos de botella.
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LoggingMiddleware> _logger;

        public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            await _next(context);
            stopwatch.Stop();

            _logger.LogInformation($"[{context.Request.Method}] {context.Request.Path} " +
                                   $"respondió con {context.Response.StatusCode} en {stopwatch.ElapsedMilliseconds}ms");
        }
    }
}
