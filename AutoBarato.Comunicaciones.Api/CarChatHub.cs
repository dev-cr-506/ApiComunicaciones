using AutoBarato.Comunicaciones.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace AutoBarato.Comunicaciones.Api
{
    public class CarChatHub : Hub
    {
        private readonly IChatService _elServicioDeChat;

        public CarChatHub(IChatService servicioDeChat)
        {
            _elServicioDeChat = servicioDeChat;
        }

        private int GetUserId2()
        {
            var elClaimDeUsuario = Context.User?.FindFirst("id_usuario")
                        ?? Context.User?.FindFirst("sub")
                        ?? throw new HubException("Claim id_usuario no encontrado.");

            return int.Parse(elClaimDeUsuario.Value);
        }

        // Cliente: connection.invoke("JoinConversation", conversationId)
        public async Task JoinConversation(string idConversacion, int idUsuario)
        {
            if (!Guid.TryParse(idConversacion, out var elIdDeConversacion))
            {
                throw new HubException($"Id de conversación inválido: {idConversacion}");
            }

            var laConversacion = await _elServicioDeChat.GetConversationByIdAsync(elIdDeConversacion, idUsuario);

            if (laConversacion == null)
            {
                throw new HubException("No tiene acceso a esta conversación.");
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, idConversacion);
        }

        // Cliente: connection.invoke("LeaveConversation", joinedConversationId)
        public async Task LeaveConversation(string idConversacion)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, idConversacion);
        }

        // Cliente: connection.invoke("SendMessage", dto)
        public async Task SendMessage(SendChatMessageDto mensajeDto)
        {
            var elIdUsuario = mensajeDto.UserId;

            if (mensajeDto.TipoMensaje == "TEXT" && string.IsNullOrWhiteSpace(mensajeDto.Text))
            {
                throw new HubException("Texto requerido para mensajes TEXT.");
            }

            if (mensajeDto.TipoMensaje != "TEXT" && string.IsNullOrWhiteSpace(mensajeDto.MediaUrl))
            {
                throw new HubException("MediaUrl requerido para mensajes multimedia.");
            }

            var elMensaje = await _elServicioDeChat.SaveMessageAsync(
                mensajeDto.ConversationId,
                elIdUsuario,
                mensajeDto.Text,
                mensajeDto.TipoMensaje,
                mensajeDto.MediaUrl,
                mensajeDto.MediaThumbnailUrl
            );

            await Clients.Group(mensajeDto.ConversationId.ToString())
                .SendAsync("ReceiveMessage", elMensaje);
        }

        // Cliente: connection.invoke("EditMessage", dto)
        public async Task EditMessage(EditChatMessageDto edicionDto)
        {
            var elIdUsuario = edicionDto.UserId;

            var elMensajeActualizado = await _elServicioDeChat.EditMessageAsync(
                edicionDto.MessageId,
                elIdUsuario,
                edicionDto.NewText
            );

            if (elMensajeActualizado == null)
            {
                throw new HubException("Mensaje no encontrado o no permitido.");
            }

            await Clients.Group(elMensajeActualizado.ConversationId.ToString())
                .SendAsync("MessageEdited", new
                {
                    id = elMensajeActualizado.Id,
                    newText = elMensajeActualizado.Texto,
                    esEditado = elMensajeActualizado.EsEditado,
                    editedAt = elMensajeActualizado.FechaEdicion
                });
        }

        // Cliente: connection.invoke("MarkRead", selectedId, null, UserId)
        public async Task MarkRead(string idConversacion, IEnumerable<Guid>? idsDeMensajes, int idUsuario)
        {
            if (!Guid.TryParse(idConversacion, out var elIdDeConversacion))
            {
                throw new HubException($"Id de conversación inválido: {idConversacion}");
            }

            await _elServicioDeChat.MarkAsReadAsync(elIdDeConversacion, idUsuario, idsDeMensajes);

            await Clients.Group(idConversacion)
                .SendAsync("MessagesRead", new
                {
                    conversationId = elIdDeConversacion,
                    UserId = idUsuario,
                    messageIds = idsDeMensajes
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