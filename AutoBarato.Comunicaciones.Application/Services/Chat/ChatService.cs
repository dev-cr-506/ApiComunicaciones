using AutoBarato.Comunicaciones.Application.DTOs.Response.Comunicaciones;
using AutoBarato.Comunicaciones.Application.Interfaces;
using AutoBarato.Comunicaciones.Domain.Interfaces.Chat;
using AutoMapper;

namespace AutoBarato.Comunicaciones.Application.Services.Chat
{
    public class ChatService : IChatService
    {
        private readonly IChatRepository _elRepositorioDeChat;
        private readonly IMapper _elMapeador;

        public ChatService(IChatRepository repositorioDeChat, IMapper mapeador)
        {
            _elRepositorioDeChat = repositorioDeChat;
            _elMapeador = mapeador;
        }

        public async Task<ChatConversacionResponse> CreateOrGetConversationAsync(
            int idAuto,
            int idVendedor,
            int idComprador)
        {
            var laConversacion = await _elRepositorioDeChat.CreateOrGetConversationAsync(
                idAuto,
                idVendedor,
                idComprador);

            if (laConversacion == null)
            {
                throw new InvalidOperationException("No se pudo crear u obtener la conversación.");
            }

            return _elMapeador.Map<ChatConversacionResponse>(laConversacion);
        }

        public async Task<IReadOnlyList<ChatConversacionResponse>> GetUserConversationsAsync(
            int idUsuario,
            int skip,
            int take)
        {
            var lasConversaciones = await _elRepositorioDeChat.GetUserConversationsAsync(
                idUsuario,
                skip,
                take
            );

            return _elMapeador.Map<List<ChatConversacionResponse>>(lasConversaciones);
        }

        public async Task<ChatConversacionResponse?> GetConversationByIdAsync(
            Guid idConversacion,
            int idUsuario)
        {
            var laConversacion = await _elRepositorioDeChat.GetConversationByIdAsync(idConversacion);

            if (laConversacion == null)
            {
                return null;
            }

            if (laConversacion.IdComprador != idUsuario && laConversacion.IdVendedor != idUsuario)
            {
                return null;
            }

            return _elMapeador.Map<ChatConversacionResponse>(laConversacion);
        }

        public async Task<IReadOnlyList<ChatMensajeResponse>> GetMessagesAsync(
            Guid idConversacion,
            int skip,
            int take)
        {
            var losMensajes = await _elRepositorioDeChat.GetMessagesByConversationAsync(
                idConversacion,
                skip,
                take
            );

            return _elMapeador.Map<List<ChatMensajeResponse>>(losMensajes);
        }

        public async Task<ChatMensajeResponse> SaveMessageAsync(
            Guid idConversacion,
            int remitenteId,
            string? texto,
            string tipoMensaje,
            string? mediaUrl,
            string? mediaThumbnailUrl)
        {
            DateTime? laFechaDeExpiracionDelMedio = null;

            var elMensaje = await _elRepositorioDeChat.InsertMessageAsync(
                idConversacion,
                remitenteId,
                texto,
                tipoMensaje,
                mediaUrl,
                mediaThumbnailUrl,
                laFechaDeExpiracionDelMedio);

            return _elMapeador.Map<ChatMensajeResponse>(elMensaje);
        }

        public async Task<ChatMensajeResponse?> EditMessageAsync(
            Guid idMensaje,
            int remitenteId,
            string textoNuevo)
        {
            if (string.IsNullOrWhiteSpace(textoNuevo))
            {
                throw new ArgumentException("El texto editado no puede estar vacío.", nameof(textoNuevo));
            }

            var elMensajeActualizado = await _elRepositorioDeChat.EditMessageAsync(
                idMensaje,
                remitenteId,
                textoNuevo);

            return _elMapeador.Map<ChatMensajeResponse>(elMensajeActualizado);
        }

        public async Task MarkAsReadAsync(
            Guid idConversacion,
            int idUsuario,
            IEnumerable<Guid>? idsDeMensajes)
        {
            await _elRepositorioDeChat.MarkMessagesAsReadAsync(
                idConversacion,
                idUsuario,
                idsDeMensajes);
        }
    }
}