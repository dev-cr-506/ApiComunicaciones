using AutoBarato.Comunicaciones.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace AutoBarato.Comunicaciones.Api
{
    public class CarChatHub : Hub
    {
        private readonly IChatService _chatService;

        public CarChatHub(IChatService chatService)
        {
            _chatService = chatService;
        }


        private int GetUserId2()
        {
            var claim = Context.User?.FindFirst("id_usuario")
                        ?? Context.User?.FindFirst("sub")
                        ?? throw new HubException("Claim id_usuario no encontrado.");
            return int.Parse(claim.Value);
        }

        // Cliente: connection.invoke("JoinConversation", conversationId)
        public async Task JoinConversation(string conversationId, int UserId)
        {
            if (!Guid.TryParse(conversationId, out var conversationGuid))
            {
                throw new HubException($"Id de conversación inválido: {conversationId}");
            }

            var conv = await _chatService.GetConversationByIdAsync(conversationGuid, UserId);
            if (conv == null)
                throw new HubException("No tiene acceso a esta conversación.");

            await Groups.AddToGroupAsync(Context.ConnectionId, conversationId);
        }

        // Cliente: connection.invoke("LeaveConversation", joinedConversationId)
        public async Task LeaveConversation(string conversationId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, conversationId);
        }

        // Cliente: connection.invoke("SendMessage", dto)  ← NO lo toco, ya te funciona
        public async Task SendMessage(SendChatMessageDto dto)
        {
            var userId = dto.UserId;

            if (dto.TipoMensaje == "TEXT" && string.IsNullOrWhiteSpace(dto.Text))
                throw new HubException("Texto requerido para mensajes TEXT.");

            if (dto.TipoMensaje != "TEXT" && string.IsNullOrWhiteSpace(dto.MediaUrl))
                throw new HubException("MediaUrl requerido para mensajes multimedia.");

            var msg = await _chatService.SaveMessageAsync(
                dto.ConversationId,
                userId,
                dto.Text,
                dto.TipoMensaje,
                dto.MediaUrl,
                dto.MediaThumbnailUrl
            );

            await Clients.Group(dto.ConversationId.ToString())
                .SendAsync("ReceiveMessage", msg);
        }

        // Cliente: connection.invoke("EditMessage", dto)
        public async Task EditMessage(EditChatMessageDto dto)
        {
            var userId = dto.UserId;

            var updated = await _chatService.EditMessageAsync(dto.MessageId, userId, dto.NewText);
            if (updated == null)
                throw new HubException("Mensaje no encontrado o no permitido.");

            await Clients.Group(updated.ConversationId.ToString())
                .SendAsync("MessageEdited", new
                {
                    id = updated.Id,
                    newText = updated.Texto,
                    esEditado = updated.EsEditado,
                    editedAt = updated.FechaEdicion
                });
        }

        // Cliente: connection.invoke("MarkRead", selectedId, null, UserId)
        public async Task MarkRead(string conversationId, IEnumerable<Guid>? messageIds, int UserId)
        {
            if (!Guid.TryParse(conversationId, out var conversationGuid))
            {
                throw new HubException($"Id de conversación inválido: {conversationId}");
            }

            await _chatService.MarkAsReadAsync(conversationGuid, UserId, messageIds);

            await Clients.Group(conversationId)
                .SendAsync("MessagesRead", new
                {
                    conversationId = conversationGuid,
                    UserId,
                    messageIds
                });
        }
    }
}


public class SendChatMessageDto
{
    public Guid ConversationId { get; set; }
    public int UserId { get; set; }
    public string? Text { get; set; }
    public string TipoMensaje { get; set; } = "TEXT"; // TEXT | IMAGE | VIDEO
    public string? MediaUrl { get; set; }
    public string? MediaThumbnailUrl { get; set; }
}

public class EditChatMessageDto
{
    public Guid MessageId { get; set; }
    public int UserId { get; set; }

    public string NewText { get; set; } = default!;
}
