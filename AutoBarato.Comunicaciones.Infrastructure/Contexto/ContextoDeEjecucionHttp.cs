using AutoBarato.Comunicaciones.Application.Interfaces.Contexto;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;

namespace AutoBarato.Comunicaciones.Infrastructure.Contexto
{
    public class ContextoDeEjecucionHttp : IContextoDeEjecucion
    {
        private readonly IHttpContextAccessor _elAccesorDeContextoHttp;

        public ContextoDeEjecucionHttp(IHttpContextAccessor accesorDeContextoHttp)
        {
            _elAccesorDeContextoHttp = accesorDeContextoHttp;
        }

        public string? ObtenerTraceId()
        {
            var elContextoHttp = _elAccesorDeContextoHttp.HttpContext;
            return Activity.Current?.TraceId.ToString() ?? elContextoHttp?.TraceIdentifier;
        }

        public string? ObtenerIdDeCorrelacion()
        {
            var elContextoHttp = _elAccesorDeContextoHttp.HttpContext;

            return elContextoHttp?.Request.Headers.TryGetValue("X-Correlation-Id", out var encabezadoDeCorrelacion) == true
                ? encabezadoDeCorrelacion.ToString()
                : null;
        }
    }
}
