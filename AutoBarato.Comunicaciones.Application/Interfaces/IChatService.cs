using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoBarato.Comunicaciones.Application.DTOs.Response.Comunicaciones;
using Microsoft.AspNetCore.Http;

namespace AutoBarato.Comunicaciones.Application.Interfaces
{
    public interface IChatService
    {
        Task<ChatConversacionResponse> CreateOrGetConversationAsync(int idAuto, int idVendedor, int idComprador);

        Task<IReadOnlyList<ChatConversacionResponse>> GetUserConversationsAsync(int idUsuario, int skip, int take);

        Task<ChatConversacionResponse?> GetConversationByIdAsync(Guid conversationId, int userId);

        Task<IReadOnlyList<ChatMensajeResponse>> GetMessagesAsync(Guid conversationId, int skip, int take);

        Task<ChatMensajeResponse> SaveMessageAsync(
            Guid conversationId,
            int remitenteId,
            string? texto,
            string tipoMensaje,
            string? mediaUrl,
            string? mediaThumbnailUrl);

        Task<ChatMensajeResponse?> EditMessageAsync(Guid messageId, int remitenteId, string newText);

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






