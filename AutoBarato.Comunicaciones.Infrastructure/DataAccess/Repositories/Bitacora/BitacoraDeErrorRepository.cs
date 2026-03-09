using AutoBarato.Comunicaciones.Domain.Entities.Bitacora;
using AutoBarato.Comunicaciones.Domain.Interfaces.Bitacora;
using AutoBarato.Comunicaciones.Infrastructure.DataAccess;
using Microsoft.Data.SqlClient;

namespace AutoBarato.ServiciosAutomotrices.Infrastructure.DataAccess.Repositories.Bitacora
{
    public sealed class BitacoraDeErrorRepository : IBitacoraDeErrorRepository
    {
        private readonly EjecutorDeProcedimientosAlmacenados _elEjecutorDeProcedimientosAlmacenados;

        public BitacoraDeErrorRepository(EjecutorDeProcedimientosAlmacenados elEjecutorDeProcedimientosAlmacenados)
        {
            _elEjecutorDeProcedimientosAlmacenados = elEjecutorDeProcedimientosAlmacenados;
        }

        public async Task InsertarErrorEnBitacoraAsync(BitacoraDeError errorDeBitacora, CancellationToken tokenCancelacion = default)
        {
            var losParametrosDelProcedimiento = ConstruirParametrosDeErrorDeBitacora(errorDeBitacora);
            await _elEjecutorDeProcedimientosAlmacenados.EjecutarProcedimientoAlmacenadoSinResultadoAsync("bitacora.Insertar_Bitacora_Error", losParametrosDelProcedimiento);
        }

        public async Task InsertarLoteDeErroresAsync(IReadOnlyCollection<BitacoraDeError> loteDeErroresDeBitacora, CancellationToken tokenCancelacion = default)
        {
            // Alternativa 1 (recomendada): SP que reciba JSON
            // Alternativa 2: Insertar en loop (más simple, menos eficiente)

            // ✅ Alternativa 1: JSON
            var elJsonDeErroresDeBitacora = System.Text.Json.JsonSerializer.Serialize(loteDeErroresDeBitacora);
            var losParametrosDelProcedimiento = new[]
            {
                new SqlParameter("@Json", elJsonDeErroresDeBitacora)
            };
            await _elEjecutorDeProcedimientosAlmacenados.EjecutarProcedimientoAlmacenadoSinResultadoAsync("bitacora.Insertar_Bitacora_Error_Batch", losParametrosDelProcedimiento);
        }

        private static SqlParameter[] ConstruirParametrosDeErrorDeBitacora(BitacoraDeError e)
        {
            return new[]
            {
                new SqlParameter("@be_modulo", e.Modulo),
                new SqlParameter("@be_servicio", (object?)e.Servicio ?? DBNull.Value),
                new SqlParameter("@be_ambiente", (object?)e.Ambiente ?? DBNull.Value),
                new SqlParameter("@be_severidad", e.Severidad),

                new SqlParameter("@be_trace_id", (object?)e.TraceId ?? DBNull.Value),
                new SqlParameter("@be_correlation_id", (object?)e.CorrelationId ?? DBNull.Value),
                new SqlParameter("@be_request_id", (object?)e.RequestId ?? DBNull.Value),

                new SqlParameter("@be_http_method", (object?)e.HttpMethod ?? DBNull.Value),
                new SqlParameter("@be_request_path", (object?)e.RequestPath ?? DBNull.Value),
                new SqlParameter("@be_query_string", (object?)e.QueryString ?? DBNull.Value),
                new SqlParameter("@be_remote_ip", (object?)e.RemoteIp ?? DBNull.Value),
                new SqlParameter("@be_user_agent", (object?)e.UserAgent ?? DBNull.Value),

                new SqlParameter("@be_id_usuario", (object?)e.IdUsuario ?? DBNull.Value),
                new SqlParameter("@be_entidad_tipo", (object?)e.EntidadTipo ?? DBNull.Value),
                new SqlParameter("@be_entidad_id", (object?)e.EntidadId ?? DBNull.Value),
                new SqlParameter("@be_reference_id", (object?)e.ReferenceId ?? DBNull.Value),
                new SqlParameter("@be_proveedor", (object?)e.Proveedor ?? DBNull.Value),

                new SqlParameter("@be_exception_type", e.ExceptionType),
                new SqlParameter("@be_message", e.Message),
                new SqlParameter("@be_stacktrace", (object?)e.StackTrace ?? DBNull.Value),
                new SqlParameter("@be_inner_message", (object?)e.InnerMessage ?? DBNull.Value),

                new SqlParameter("@be_metadata_json", (object?)e.MetadataJson ?? DBNull.Value),

                new SqlParameter("@be_id_usuario_reg", e.IdUsuarioReg),
            };
        }
    }
}
