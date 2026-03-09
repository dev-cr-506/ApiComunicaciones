using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoBarato.Comunicaciones.Domain.Entities.Comunicaciones;

namespace AutoBarato.Comunicaciones.Domain.Interfaces.Chat
{
    public interface IChatRepository
    {
        Task<ChatConversacion?> CreateOrGetConversationAsync(
           int idAuto,
           int idVendedor,
           int idComprador);

        Task<IReadOnlyList<ChatConversacion>> GetUserConversationsAsync(
            int idUsuario,
            int skip,
            int take);

        Task<ChatConversacion?> GetConversationByIdAsync(Guid conversationId);

        Task<IReadOnlyList<ChatMensaje>> GetMessagesByConversationAsync(
            Guid conversationId,
            int skip,
            int take);

        Task<ChatMensaje> InsertMessageAsync(
            Guid conversationId,
            int remitenteId,
            string? texto,
            string tipoMensaje,
            string? mediaUrl,
            string? mediaThumbnailUrl,
            DateTime? mediaExpiraEn);

        Task<ChatMensaje?> EditMessageAsync(
            Guid messageId,
            int remitenteId,
            string newText);

        Task MarkMessagesAsReadAsync(
            Guid conversationId,
            int userId,
            IEnumerable<Guid>? messageIds);
    }
}
