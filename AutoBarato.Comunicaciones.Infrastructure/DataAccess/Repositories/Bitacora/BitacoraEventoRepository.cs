using AutoBarato.Comunicaciones.Domain.Entities.Bitacora;
using AutoBarato.Comunicaciones.Domain.Interfaces.Bitacora;
using AutoBarato.Comunicaciones.Infrastructure.DataAccess;
using Microsoft.Data.SqlClient;

namespace AutoBarato.ServiciosAutomotrices.Infrastructure.DataAccess.Repositories.Bitacora
{
    public sealed class BitacoraEventoRepository : IBitacoraEventoRepository
    {
        private readonly EjecutorDeProcedimientosAlmacenados _elEjecutorDeProcedimientosAlmacenados;

        public BitacoraEventoRepository(EjecutorDeProcedimientosAlmacenados ejecutorDeProcedimientosAlmacenados)
        {
            _elEjecutorDeProcedimientosAlmacenados = ejecutorDeProcedimientosAlmacenados;
        }

        public async Task InsertarEventoAsync(BitacoraEvento eventoDeBitacora, CancellationToken tokenCancelacion = default)
        {
            var losParametrosDelProcedimiento = ConstruirParametrosDeEventoDeBitacora(eventoDeBitacora);
            await _elEjecutorDeProcedimientosAlmacenados.EjecutarProcedimientoAlmacenadoSinResultadoAsync("bitacora.Insertar_Bitacora_Evento", losParametrosDelProcedimiento);
        }

        public async Task InsertarLoteDeEventosAsync(IReadOnlyCollection<BitacoraEvento> loteDeEventosDeBitacora, CancellationToken tokenCancelacion = default)
        {
            var elJsonDeEventosDeBitacora = System.Text.Json.JsonSerializer.Serialize(loteDeEventosDeBitacora);
            var losParametrosDelProcedimiento = new[]
            {
                new SqlParameter("@Json", elJsonDeEventosDeBitacora)
            };

            await _elEjecutorDeProcedimientosAlmacenados.EjecutarProcedimientoAlmacenadoSinResultadoAsync("bitacora.Insertar_Bitacora_Evento_Batch", losParametrosDelProcedimiento);
        }

        private static SqlParameter[] ConstruirParametrosDeEventoDeBitacora(BitacoraEvento eventoDeBitacora)
        {
            return new[]
            {
                new SqlParameter("@bv_modulo", eventoDeBitacora.Modulo),
                new SqlParameter("@bv_servicio", (object?)eventoDeBitacora.Servicio ?? DBNull.Value),
                new SqlParameter("@bv_ambiente", (object?)eventoDeBitacora.Ambiente ?? DBNull.Value),

                new SqlParameter("@bv_tipo", eventoDeBitacora.Tipo),
                new SqlParameter("@bv_nivel", eventoDeBitacora.Nivel),

                new SqlParameter("@bv_mensaje", (object?)eventoDeBitacora.Mensaje ?? DBNull.Value),

                new SqlParameter("@bv_trace_id", (object?)eventoDeBitacora.TraceId ?? DBNull.Value),
                new SqlParameter("@bv_correlation_id", (object?)eventoDeBitacora.CorrelationId ?? DBNull.Value),

                new SqlParameter("@bv_id_usuario", (object?)eventoDeBitacora.IdUsuario ?? DBNull.Value),

                new SqlParameter("@bv_entidad_tipo", (object?)eventoDeBitacora.EntidadTipo ?? DBNull.Value),
                new SqlParameter("@bv_entidad_id", (object?)eventoDeBitacora.EntidadId ?? DBNull.Value),

                new SqlParameter("@bv_reference_id", (object?)eventoDeBitacora.ReferenceId ?? DBNull.Value),
                new SqlParameter("@bv_proveedor", (object?)eventoDeBitacora.Proveedor ?? DBNull.Value),

                new SqlParameter("@bv_payload_json", (object?)eventoDeBitacora.PayloadJson ?? DBNull.Value),

                new SqlParameter("@bv_id_usuario_reg", eventoDeBitacora.IdUsuarioReg),
            };
        }
    }
}
