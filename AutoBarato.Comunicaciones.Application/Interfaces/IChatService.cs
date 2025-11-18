using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoBarato.Comunicaciones.Application.DTOs;
using Microsoft.AspNetCore.Http;

namespace AutoBarato.Comunicaciones.Application.Interfaces
{
    public interface IChatService
    {
        Task<ChatConversationDto> CreateOrGetConversationAsync(int idAuto, int idVendedor, int idComprador);

        Task<IReadOnlyList<ChatConversationDto>> GetUserConversationsAsync(int idUsuario, int skip, int take);

        Task<ChatConversationDto?> GetConversationByIdAsync(Guid conversationId, int userId);

        Task<IReadOnlyList<ChatMessageDto>> GetMessagesAsync(Guid conversationId, int skip, int take);

        Task<ChatMessageDto> SaveMessageAsync(
            Guid conversationId,
            int remitenteId,
            string? texto,
            string tipoMensaje,
            string? mediaUrl,
            string? mediaThumbnailUrl);

        Task<ChatMessageDto?> EditMessageAsync(Guid messageId, int remitenteId, string newText);

        Task MarkAsReadAsync(Guid conversationId, int userId, IEnumerable<Guid>? messageIds);
    }

    //public interface IChatMediaService
    //{
    //    Task<ChatMediaResult> UploadAsync(
    //        IFormFile file,
    //        int userId,
    //        Guid? conversationId = null);
    //}

    public record ChatMediaResult(
        string MediaUrl,
        string? ThumbnailUrl,
        string MediaType // "IMAGE" | "VIDEO"
    );
}






