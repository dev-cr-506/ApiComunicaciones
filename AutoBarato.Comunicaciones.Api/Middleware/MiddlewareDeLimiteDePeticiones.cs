using Microsoft.Extensions.Caching.Memory;

namespace AutoBarato.Comunicaciones.Api.Middleware
{
    public class MiddlewareDeLimiteDePeticiones
    {
        private readonly RequestDelegate _elSiguienteMiddleware;
        private readonly IMemoryCache _elCacheEnMemoria;

        public MiddlewareDeLimiteDePeticiones(RequestDelegate siguienteMiddleware, IMemoryCache cacheEnMemoria)
        {
            _elSiguienteMiddleware = siguienteMiddleware;
            _elCacheEnMemoria = cacheEnMemoria;
        }

        public async Task Invoke(HttpContext contextoHttp)
        {
            string laLlaveDeLimiteDePeticiones = $"RateLimit-{contextoHttp.Connection.RemoteIpAddress}";

            if (_elCacheEnMemoria.TryGetValue(laLlaveDeLimiteDePeticiones, out int laCantidadDePeticiones))
            {
                if (laCantidadDePeticiones >= 10) // Máximo 10 peticiones por IP en 1 minuto
                {
                    contextoHttp.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    await contextoHttp.Response.WriteAsync("Has excedido el límite de peticiones. Intenta más tarde.");
                    return;
                }
                _elCacheEnMemoria.Set(laLlaveDeLimiteDePeticiones, laCantidadDePeticiones + 1, TimeSpan.FromMinutes(1));
            }
            else
            {
                _elCacheEnMemoria.Set(laLlaveDeLimiteDePeticiones, 1, TimeSpan.FromMinutes(1));
            }

            await _elSiguienteMiddleware(contextoHttp);
        }
    }
}
