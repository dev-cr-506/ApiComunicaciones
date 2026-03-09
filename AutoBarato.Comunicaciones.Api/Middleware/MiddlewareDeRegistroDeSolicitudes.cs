using System.Diagnostics;

namespace AutoBarato.Comunicaciones.Api.Middleware
{
    public class MiddlewareDeRegistroDeSolicitudes
    {
        private readonly RequestDelegate _elSiguienteMiddleware;
        private readonly ILogger<MiddlewareDeRegistroDeSolicitudes> _elRegistrador;

        public MiddlewareDeRegistroDeSolicitudes(RequestDelegate siguienteMiddleware, ILogger<MiddlewareDeRegistroDeSolicitudes> registrador)
        {
            _elSiguienteMiddleware = siguienteMiddleware;
            _elRegistrador = registrador;
        }

        public async Task Invoke(HttpContext contextoHttp)
        {
            var elCronometro = Stopwatch.StartNew();
            try
            {
                await _elSiguienteMiddleware(contextoHttp);
            }
            finally
            {
                elCronometro.Stop();
                _elRegistrador.LogInformation("[{Method}] {Path} respondió con {StatusCode} en {Elapsed}ms",
                    contextoHttp.Request.Method,
                    contextoHttp.Request.Path,
                    contextoHttp.Response?.StatusCode,
                    elCronometro.ElapsedMilliseconds);
            }
        }
    }
}
