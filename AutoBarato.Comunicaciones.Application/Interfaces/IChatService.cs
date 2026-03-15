
using AutoBarato.Comunicaciones.Application.DTOs.Response.Comunicaciones;


namespace AutoBarato.Comunicaciones.Application.Interfaces
{
    public interface IChatService
    {
        Task<ChatConversacionResponse> CrearOObtenerConversacionAsync(int idAuto, int idVendedor, int idComprador);

        Task<IReadOnlyList<ChatConversacionResponse>> ObtenerMisConversacionesAsync(int idUsuario, int skip, int take);

        Task<ChatConversacionResponse?> ObtenerConversacionPorIdAsync(Guid conversationId, int userId);

        Task<IReadOnlyList<ChatMensajeResponse>> ObtenerMensajesAsync(Guid conversationId, int skip, int take);

        Task<ChatMensajeResponse> GuardarMensajeAsync(
            Guid conversationId,
            int remitenteId,
            string? texto,
            string tipoMensaje,
            string? mediaUrl,
            string? mediaThumbnailUrl);

        Task<ChatMensajeResponse?> EditarMensajeAsync(Guid messageId, int remitenteId, string newText);

        Task MarcarComoLeidoAsync(Guid conversationId, int userId, IEnumerable<Guid>? messageIds);
    }



}






