using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using AutoBarato.Comunicaciones.Domain.Entities;
using AutoBarato.Comunicaciones.Domain.Interfaces;
using Microsoft.Data.SqlClient;

namespace AutoBarato.Comunicaciones.Infrastructure.DataAccess.Repositories
{
    public class ChatRepository : IChatRepository
    {
        private readonly StoredProcedureExecutor _spExecutor;

        public ChatRepository(StoredProcedureExecutor spExecutor)
        {
            _spExecutor = spExecutor;
        }

        public async Task<ChatConversation?> CreateOrGetConversationAsync(
            int idAuto,
            int idVendedor,
            int idComprador)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "@id_auto", idAuto },
                    { "@c_vendedor", idVendedor },
                    { "@c_comprador", idComprador }
                };

                var result = await _spExecutor.ExecuteStoredProcedureSingleAsync<ChatConversation>(
                    "comunicaciones.Crear_Obtener_Conversacion",
                    parameters);

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al crear/obtener conversación en BD", ex);
            }
        }

        public async Task<IReadOnlyList<ChatConversation>> GetUserConversationsAsync(
         int idUsuario,
         int skip,
         int take)
        {
            try
            {
                var parameters = new[]
                {
            new SqlParameter("@id_usuario", SqlDbType.Int) { Value = idUsuario },
            new SqlParameter("@skip", SqlDbType.Int)       { Value = skip },
            new SqlParameter("@take", SqlDbType.Int)       { Value = take }
        };

                var list = await _spExecutor.ExecuteStoredProcedureAsync<ChatConversation>(
                    "comunicaciones.Consultar_Conversaciones_Usuario",
                    parameters);

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al consultar conversaciones de usuario en BD", ex);
            }
        }


        public async Task<ChatConversation?> GetConversationByIdAsync(Guid conversationId)
        {
            try
            {
                var result = await _spExecutor.ExecuteStoredProcedureSingleAsync<ChatConversation>(
                    "comunicaciones.Consultar_Conversacion_Id",
                    conversationId // <- se convierte en @p0
                );

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al consultar conversación por Id en BD", ex);
            }
        }


        public async Task<IReadOnlyList<ChatMessage>> GetMessagesByConversationAsync(
            Guid conversationId,
            int skip,
            int take)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@id_conversacion", SqlDbType.UniqueIdentifier)
                    {
                        Value = conversationId
                    },
                    new SqlParameter("@skip", SqlDbType.Int) { Value = skip },
                    new SqlParameter("@take", SqlDbType.Int) { Value = take }
                };

                var list = await _spExecutor.ExecuteStoredProcedureAsync<ChatMessage>(
                    "comunicaciones.Listar_Mensajes_Conversacion_Paginado",
                    parameters);

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al consultar mensajes de conversación en BD", ex);
            }
        }

        public async Task<ChatMessage> InsertMessageAsync(
            Guid conversationId,
            int remitenteId,
            string? texto,
            string tipoMensaje,
            string? mediaUrl,
            string? mediaThumbnailUrl,
            DateTime? mediaExpiraEn)
        {
            try
            {
                var parameters = new Dictionary<string, object?>
                {
                    { "@id_conversacion", conversationId },
                    { "@m_remitente", remitenteId },
                    { "@m_texto", (object?)texto ?? DBNull.Value },
                    { "@m_tipo_mensaje", tipoMensaje },
                    { "@m_media_url", (object?)mediaUrl ?? DBNull.Value },
                    { "@m_media_thumbnail_url", (object?)mediaThumbnailUrl ?? DBNull.Value },
                    { "@m_media_expira_en", (object?)mediaExpiraEn ?? DBNull.Value }
                };

                var result = await _spExecutor.ExecuteStoredProcedureSingleAsync<ChatMessage>(
                    "comunicaciones.Insertar_Mensaje",
                    parameters);

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al insertar mensaje en BD", ex);
            }
        }

        public async Task<ChatMessage?> EditMessageAsync(
            Guid messageId,
            int remitenteId,
            string newText)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@id_mensaje", SqlDbType.UniqueIdentifier)
                    {
                        Value = messageId
                    },
                    new SqlParameter("@m_remitente", SqlDbType.Int)
                    {
                        Value = remitenteId
                    },
                    new SqlParameter("@nuevo_texto", SqlDbType.NVarChar, 1000)
                    {
                        Value = (object)newText ?? DBNull.Value
                    }
                };

                var result = await _spExecutor.ExecuteStoredProcedureSingleAsync<ChatMessage>(
                    "comunicaciones.Editar_Mensaje",
                    parameters);

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al editar mensaje en BD", ex);
            }
        }

        public async Task MarkMessagesAsReadAsync(
            Guid conversationId,
            int userId,
            IEnumerable<Guid>? messageIds)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@id_conversacion", SqlDbType.UniqueIdentifier)
                    {
                        Value = conversationId
                    },
                    new SqlParameter("@id_usuario", SqlDbType.Int)
                    {
                        Value = userId
                    }
                };

                await _spExecutor.ExecuteNonQueryAsync(
                    "comunicaciones.Marcar_Mensajes_Leidos",
                    parameters);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al marcar mensajes como leídos en BD", ex);
            }
        }
    }
}
