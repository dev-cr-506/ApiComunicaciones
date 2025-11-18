using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoBarato.Comunicaciones.Domain.Entities;

namespace AutoBarato.Comunicaciones.Domain.Interfaces
{
    public interface IChatRepository
    {
        Task<ChatConversation?> CreateOrGetConversationAsync(
           int idAuto,
           int idVendedor,
           int idComprador);

        Task<IReadOnlyList<ChatConversation>> GetUserConversationsAsync(
            int idUsuario,
            int skip,
            int take);

        Task<ChatConversation?> GetConversationByIdAsync(Guid conversationId);

        Task<IReadOnlyList<ChatMessage>> GetMessagesByConversationAsync(
            Guid conversationId,
            int skip,
            int take);

        Task<ChatMessage> InsertMessageAsync(
            Guid conversationId,
            int remitenteId,
            string? texto,
            string tipoMensaje,
            string? mediaUrl,
            string? mediaThumbnailUrl,
            DateTime? mediaExpiraEn);

        Task<ChatMessage?> EditMessageAsync(
            Guid messageId,
            int remitenteId,
            string newText);

        Task MarkMessagesAsReadAsync(
            Guid conversationId,
            int userId,
            IEnumerable<Guid>? messageIds);
    }
}
