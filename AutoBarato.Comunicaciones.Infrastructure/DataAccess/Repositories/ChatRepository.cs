using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using AutoBarato.Comunicaciones.Domain.Entities.Comunicaciones;
using AutoBarato.Comunicaciones.Domain.Interfaces.Chat;
using Microsoft.Data.SqlClient;

namespace AutoBarato.Comunicaciones.Infrastructure.DataAccess.Repositories
{
    public class ChatRepository : IChatRepository
    {
        private readonly EjecutorDeProcedimientosAlmacenados _elEjecutorDeProcedimientosAlmacenados;

        public ChatRepository(EjecutorDeProcedimientosAlmacenados ejecutorDeProcedimientosAlmacenados)
        {
            _elEjecutorDeProcedimientosAlmacenados = ejecutorDeProcedimientosAlmacenados;
        }

        public async Task<ChatConversacion?> CreateOrGetConversationAsync(
            int idAuto,
            int idVendedor,
            int idComprador)
        {
            var losParametros = new[]
            {
                new SqlParameter("@id_auto", SqlDbType.Int) { Value = idAuto },
                new SqlParameter("@c_vendedor", SqlDbType.Int) { Value = idVendedor },
                new SqlParameter("@c_comprador", SqlDbType.Int) { Value = idComprador }
            };

            var elResultado = await _elEjecutorDeProcedimientosAlmacenados.EjecutarProcedimientoAlmacenadoUnicoAsync<ChatConversacion>(
                "comunicaciones.Crear_Obtener_Conversacion",
                losParametros);

            return elResultado;
        }

        public async Task<IReadOnlyList<ChatConversacion>> GetUserConversationsAsync(
            int idUsuario,
            int skip,
            int take)
        {
            var losParametros = new[]
            {
                new SqlParameter("@id_usuario", SqlDbType.Int) { Value = idUsuario },
                new SqlParameter("@skip", SqlDbType.Int) { Value = skip },
                new SqlParameter("@take", SqlDbType.Int) { Value = take }
            };

            var laListaDeResultados = await _elEjecutorDeProcedimientosAlmacenados.EjecutarProcedimientoAlmacenadoAsync<ChatConversacion>(
                "comunicaciones.Consultar_Conversaciones_Usuario",
                losParametros);

            return laListaDeResultados;
        }

        public async Task<ChatConversacion?> GetConversationByIdAsync(Guid idConversacion)
        {
            var losParametros = new[]
            {
                new SqlParameter("@id_conversacion", SqlDbType.UniqueIdentifier) { Value = idConversacion }
            };

            var elResultado = await _elEjecutorDeProcedimientosAlmacenados.EjecutarProcedimientoAlmacenadoUnicoAsync<ChatConversacion>(
                "comunicaciones.Consultar_Conversacion_Id",
                losParametros);

            return elResultado;
        }

        public async Task<IReadOnlyList<ChatMensaje>> GetMessagesByConversationAsync(
            Guid idConversacion,
            int skip,
            int take)
        {
            var losParametros = new[]
            {
                new SqlParameter("@id_conversacion", SqlDbType.UniqueIdentifier) { Value = idConversacion },
                new SqlParameter("@skip", SqlDbType.Int) { Value = skip },
                new SqlParameter("@take", SqlDbType.Int) { Value = take }
            };

            var laListaDeResultados = await _elEjecutorDeProcedimientosAlmacenados.EjecutarProcedimientoAlmacenadoAsync<ChatMensaje>(
                "comunicaciones.Listar_Mensajes_Conversacion_Paginado",
                losParametros);

            return laListaDeResultados;
        }

        public async Task<ChatMensaje> InsertMessageAsync(
            Guid idConversacion,
            int remitenteId,
            string? texto,
            string tipoMensaje,
            string? mediaUrl,
            string? mediaThumbnailUrl,
            DateTime? fechaDeExpiracionDelMedio)
        {
            var losParametros = new[]
            {
                new SqlParameter("@id_conversacion", SqlDbType.UniqueIdentifier) { Value = idConversacion },
                new SqlParameter("@m_remitente", SqlDbType.Int) { Value = remitenteId },
                new SqlParameter("@m_texto", SqlDbType.NVarChar, -1) { Value = (object?)texto ?? DBNull.Value },
                new SqlParameter("@m_tipo_mensaje", SqlDbType.NVarChar, 50) { Value = tipoMensaje },
                new SqlParameter("@m_media_url", SqlDbType.NVarChar, -1) { Value = (object?)mediaUrl ?? DBNull.Value },
                new SqlParameter("@m_media_thumbnail_url", SqlDbType.NVarChar, -1) { Value = (object?)mediaThumbnailUrl ?? DBNull.Value },
                new SqlParameter("@m_media_expira_en", SqlDbType.DateTime2) { Value = (object?)fechaDeExpiracionDelMedio ?? DBNull.Value }
            };

            var elResultado = await _elEjecutorDeProcedimientosAlmacenados.EjecutarProcedimientoAlmacenadoUnicoAsync<ChatMensaje>(
                "comunicaciones.Insertar_Mensaje",
                losParametros);

            return elResultado;
        }

        public async Task<ChatMensaje?> EditMessageAsync(
            Guid idMensaje,
            int remitenteId,
            string textoNuevo)
        {
            var losParametros = new[]
            {
                new SqlParameter("@id_mensaje", SqlDbType.UniqueIdentifier) { Value = idMensaje },
                new SqlParameter("@m_remitente", SqlDbType.Int) { Value = remitenteId },
                new SqlParameter("@nuevo_texto", SqlDbType.NVarChar, 1000) { Value = (object)textoNuevo ?? DBNull.Value }
            };

            var elResultado = await _elEjecutorDeProcedimientosAlmacenados.EjecutarProcedimientoAlmacenadoUnicoAsync<ChatMensaje>(
                "comunicaciones.Editar_Mensaje",
                losParametros);

            return elResultado;
        }

        public async Task MarkMessagesAsReadAsync(
            Guid idConversacion,
            int idUsuario,
            IEnumerable<Guid>? idsDeMensajes)
        {
            var losParametros = new[]
            {
                new SqlParameter("@id_conversacion", SqlDbType.UniqueIdentifier) { Value = idConversacion },
                new SqlParameter("@id_usuario", SqlDbType.Int) { Value = idUsuario }
            };

            await _elEjecutorDeProcedimientosAlmacenados.EjecutarProcedimientoAlmacenadoSinResultadoAsync(
                "comunicaciones.Marcar_Mensajes_Leidos",
                losParametros);
        }
    }
}