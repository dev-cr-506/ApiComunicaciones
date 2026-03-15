
using AutoBarato.Comunicaciones.Domain.Entities.Comunicaciones;

namespace AutoBarato.Comunicaciones.Domain.Interfaces.Chat
{
    public interface IChatRepository
    {
        Task<ChatConversacion?> CrearOObtenerConversacionesAsync(
           int idAuto,
           int idVendedor,
           int idComprador);

        Task<IReadOnlyList<ChatConversacion>> ObtenerConversacionesUsuarioAsync(
            int idUsuario,
            int skip,
            int take);

        Task<ChatConversacion?> ObtenerConversacionPorIdAsync(Guid idConversacion);

        Task<IReadOnlyList<ChatMensaje>> ObtenerMensajesDeConversacionAsync(
            Guid conversationId,
            int skip,
            int take);

        Task<ChatMensaje> InsertarMensajeAsync(
            Guid conversationId,
            int remitenteId,
            string? texto,
            string tipoMensaje,
            string? mediaUrl,
            string? mediaThumbnailUrl,
            DateTime? mediaExpiraEn);

        Task<ChatMensaje?> EditarMensajeAsync(
            Guid messageId,
            int remitenteId,
            string newText);

        Task MarcarMensajeComoLeidoAsync(
            Guid conversationId,
            int userId,
            IEnumerable<Guid>? messageIds);
    }
}
