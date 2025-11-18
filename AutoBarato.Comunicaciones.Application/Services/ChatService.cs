using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoBarato.Comunicaciones.Application.DTOs;
using AutoBarato.Comunicaciones.Application.Interfaces;
using AutoBarato.Comunicaciones.Domain.Interfaces;
using AutoMapper;

namespace AutoBarato.Comunicaciones.Application.Services
{
    public class ChatService : IChatService
    {
        private readonly IChatRepository _chatRepository;
        private readonly IMapper _mapper;

        public ChatService(IChatRepository chatRepository, IMapper mapper)
        {
            _chatRepository = chatRepository;
            _mapper = mapper;
        }

        public async Task<ChatConversationDto> CreateOrGetConversationAsync(
            int idAuto,
            int idVendedor,
            int idComprador)
        {
            var conv = await _chatRepository.CreateOrGetConversationAsync(
                idAuto,
                idVendedor,
                idComprador);

            if (conv == null)
            {
                throw new InvalidOperationException("No se pudo crear u obtener la conversación.");
            }

            return _mapper.Map<ChatConversationDto>(conv);

        }

        public async Task<IReadOnlyList<ChatConversationDto>> GetUserConversationsAsync(
            int idUsuario,
            int skip,
            int take)
        {
            var conversations = await _chatRepository.GetUserConversationsAsync(
                idUsuario,
                skip,
                take
            );

            return _mapper.Map<List<ChatConversationDto>>(conversations);

        }


        public async Task<ChatConversationDto?> GetConversationByIdAsync(
            Guid conversationId,
            int userId)
        {
            var conv = await _chatRepository.GetConversationByIdAsync(conversationId);
            if (conv == null)
                return null;

            // Regla de negocio: el usuario debe pertenecer a la conversación
            if (conv.IdComprador != userId && conv.IdVendedor != userId)
                return null;

            return _mapper.Map<ChatConversationDto>(conv);
        }

        public async Task<IReadOnlyList<ChatMessageDto>> GetMessagesAsync(
         Guid conversationId,
         int skip,
         int take)
        {
            var messages = await _chatRepository.GetMessagesByConversationAsync(
                conversationId,
                skip,
                take
            );

            return _mapper.Map<List<ChatMessageDto>>(messages);

        }

        public async Task<ChatMessageDto> SaveMessageAsync(
            Guid conversationId,
            int remitenteId,
            string? texto,
            string tipoMensaje,
            string? mediaUrl,
            string? mediaThumbnailUrl)
        {
            // Aquí puedes validar reglas de negocio extra:
            // - TipoMensaje permitido
            // - Largo máximo de texto
            // - etc.

            DateTime? mediaExpiraEn = null;
            // Si quisieras que la media expire en X días:
            // if (!string.IsNullOrWhiteSpace(mediaUrl))
            //     mediaExpiraEn = DateTime.UtcNow.AddDays(30);

            var message = await _chatRepository.InsertMessageAsync(
                conversationId,
                remitenteId,
                texto,
                tipoMensaje,
                mediaUrl,
                mediaThumbnailUrl,
                mediaExpiraEn);

            return _mapper.Map<ChatMessageDto>(message);
        }

        public async Task<ChatMessageDto?> EditMessageAsync(
            Guid messageId,
            int remitenteId,
            string newText)
        {
            if (string.IsNullOrWhiteSpace(newText))
                throw new ArgumentException("El texto editado no puede estar vacío.", nameof(newText));

            // Aquí podrías agregar lógica de ventana de tiempo para edición
            // (ej: no permitir editar después de 15 minutos)

            var updated = await _chatRepository.EditMessageAsync(
                messageId,
                remitenteId,
                newText);

            return _mapper.Map<ChatMessageDto>(updated);


        }

        public async Task MarkAsReadAsync(
            Guid conversationId,
            int userId,
            IEnumerable<Guid>? messageIds)
        {
            // Por ahora el repositorio ignora messageIds y marca todos los de la conversación.
            // Luego se puede extender a TVP.

            await _chatRepository.MarkMessagesAsReadAsync(
                conversationId,
                userId,
                messageIds);
        }
    }
}
