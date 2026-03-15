using AutoBarato.Comunicaciones.Api.Models.Common;
using AutoBarato.Comunicaciones.Application.Interfaces.Bitacora;
using AutoBarato.Comunicaciones.Domain.Entities.Bitacora;
using System.Diagnostics;
using System.Net;
using System.Text.Json;

namespace AutoBarato.Comunicaciones.Api.Middleware
{
    public sealed class MiddlewareDeManejoDeExcepciones
    {
        private readonly RequestDelegate _elSiguienteMiddleware;
        private readonly ILogger<MiddlewareDeManejoDeExcepciones> _elRegistrador;
        private readonly IBitacoraErrorService _elServicioDeBitacoraDeErrores;
        private readonly IConfiguration _laConfiguracion;

        public MiddlewareDeManejoDeExcepciones
        (
            RequestDelegate siguienteMiddleware,
            ILogger<MiddlewareDeManejoDeExcepciones> registrador,
            IBitacoraErrorService servicioDeBitacoraDeErrores,
            IConfiguration configuracion
        )
        {
            _elSiguienteMiddleware = siguienteMiddleware;
            _elRegistrador = registrador;
            _elServicioDeBitacoraDeErrores = servicioDeBitacoraDeErrores;
            _laConfiguracion = configuracion;
        }

        public async Task Invoke(HttpContext contextoHttp)
        {
            try
            {
                await _elSiguienteMiddleware(contextoHttp);
            }
            catch (Exception laExcepcionNoManejada)
            {
                // 1) Siempre a ILogger
                _elRegistrador.LogError(laExcepcionNoManejada, "Error no manejado en Comunicaciones API. TraceId={TraceId}", ObtenerTraceId(contextoHttp));

                // 2) Encolar bitácora (NO await DB)
                try
                {
                    var elErrorDeBitacora = ConstruirErrorDeBitacora(contextoHttp, laExcepcionNoManejada);
                    _elServicioDeBitacoraDeErrores.AgregarErrorDeBitacoraALaCola(elErrorDeBitacora);
                }
                catch (Exception laExcepcionAlEncolarBitacora)
                {
                    _elRegistrador.LogError(laExcepcionAlEncolarBitacora, "No se pudo encolar bitácora. Se ignora para no afectar request.");
                }

                if (contextoHttp.Response.HasStarted)
                {
                    _elRegistrador.LogWarning("La respuesta ya inició; no se puede escribir payload de error. TraceId={TraceId}", ObtenerTraceId(contextoHttp));
                    throw;
                }

                // 3) Respuesta estándar
                await EscribirRespuestaDeErrorAsync(contextoHttp);
            }
        }

        private BitacoraDeError ConstruirErrorDeBitacora(HttpContext contextoHttp, Exception excepcion)
        {
            var elAmbiente = _laConfiguracion["ASPNETCORE_ENVIRONMENT"];
            var elNombreDelServicio = "AutoBarato.Comunicaciones.Api";
            var elCodigoDelModulo = "COMUNIC";

            // OJO: algunos headers pueden venir gigantes
            var elAgenteDeUsuario = contextoHttp.Request?.Headers["User-Agent"].ToString();

            return new BitacoraDeError
            {
                // NOT NULL en tabla (varchar(30))
                Modulo = ObtenerTextoNoVacioYLimitado(elCodigoDelModulo, 30, "NA"),

                // varchar(60), varchar(20)
                Servicio = LimitarTexto(elNombreDelServicio, 60),
                Ambiente = LimitarTexto(elAmbiente, 20),

                Severidad = 3,

                // varchar(64)
                TraceId = LimitarTexto(ObtenerTraceId(contextoHttp), 64),
                CorrelationId = LimitarTexto(ObtenerIdDeCorrelacion(contextoHttp), 64),
                RequestId = LimitarTexto(contextoHttp.TraceIdentifier, 64),

                // varchar(10), varchar(260), nvarchar(1000), varchar(45), nvarchar(300)
                HttpMethod = LimitarTexto(contextoHttp.Request?.Method, 10),
                RequestPath = LimitarTexto(contextoHttp.Request?.Path.Value, 260),
                QueryString = LimitarTexto(contextoHttp.Request?.QueryString.Value, 1000),
                RemoteIp = LimitarTexto(contextoHttp.Connection?.RemoteIpAddress?.ToString(), 45),
                UserAgent = LimitarTexto(elAgenteDeUsuario, 300),

                IdUsuario = 1, // TODO JWT

                // varchar(60), varchar(80), varchar(80), varchar(30)
                EntidadTipo = LimitarTexto(null, 60),
                EntidadId = LimitarTexto(null, 80),
                ReferenceId = LimitarTexto(null, 80),
                Proveedor = LimitarTexto(null, 30),

                // NOT NULL en tabla: nvarchar(200), nvarchar(1000)
                ExceptionType = ObtenerTextoNoVacioYLimitado(excepcion.GetType().FullName, 200, "Exception"),
                Message = ObtenerTextoNoVacioYLimitado(excepcion.Message, 1000, "(sin mensaje)"),

                // nvarchar(max) ok, nvarchar(1000)
                StackTrace = excepcion.StackTrace, // max -> no truncar necesario
                InnerMessage = LimitarTexto(excepcion.InnerException?.Message, 1000),

                // nvarchar(max) ok
                MetadataJson = null,

                // NOT NULL en tabla
                IdUsuarioReg = 1 // TODO JWT
            };
        }

        private static string? ObtenerIdDeCorrelacion(HttpContext contextoHttp)
        {
            if (contextoHttp.Request.Headers.TryGetValue("X-Correlation-Id", out var v))
            {
                return v.ToString();
            }

            return null;
        }

        private static string ObtenerTraceId(HttpContext contextoHttp)
        {
            // OpenTelemetry compatible
            var elTraceId = Activity.Current?.TraceId.ToString();
            return !string.IsNullOrWhiteSpace(elTraceId)
                ? elTraceId
                : contextoHttp.TraceIdentifier;
        }

        private static string? LimitarTexto(string? texto, int longitudMaxima)
        {
            if (string.IsNullOrWhiteSpace(texto))
            {
                return texto;
            }
            texto = texto.Trim();
            return texto.Length <= longitudMaxima ? texto : texto.Substring(0, longitudMaxima);
        }

        // Variante “segura” para campos NOT NULL (si querés evitar null/empty)
        private static string ObtenerTextoNoVacioYLimitado(string? texto, int longitudMaxima, string valorPorDefecto)
        {
            var elTextoLimitado = LimitarTexto(texto, longitudMaxima);
            return string.IsNullOrWhiteSpace(elTextoLimitado) ? valorPorDefecto : elTextoLimitado!;
        }

        private static async Task EscribirRespuestaDeErrorAsync(HttpContext contextoHttp)
        {
            contextoHttp.Response.ContentType = "application/json";
            contextoHttp.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var elPayloadDeRespuestaDeError = Response<object>.ErrorResponse(
                HttpStatusCode.InternalServerError,
                "Ocurrió un error en el servidor."
            );

            await contextoHttp.Response.WriteAsync(JsonSerializer.Serialize(elPayloadDeRespuestaDeError));
        }
    }
}