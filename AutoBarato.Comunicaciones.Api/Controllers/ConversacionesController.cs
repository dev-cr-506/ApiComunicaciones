using AutoBarato.Comunicaciones.Application.DTOs;
using AutoBarato.Comunicaciones.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AutoBarato.Comunicaciones.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConversacionesController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly IChatMediaService _chatMediaService;

        public ConversacionesController(IChatService chatService, IChatMediaService chatMediaService)
        {
            _chatService = chatService;
            _chatMediaService = chatMediaService;
        }

        // 👈 OJO: atributo totalmente calificado para evitar confusiones
        [Microsoft.AspNetCore.Mvc.NonAction]
        protected int GetUserId()
        {
            var usuarioClaim = User.FindFirst("IdUsuario")?.Value;
            // Usa tu helper
            return Utils.Utils.ObtenerUsuarioDesdeToken(Request.Headers.Authorization);
        }

        public class CreateConversationRequest
        {
            public int IdAuto { get; set; }
            public int IdVendedor { get; set; }
        }

        [HttpPost("CreateOrGetConversation")]
        public async Task<ActionResult<ChatConversationDto>> CreateOrGetConversation(
            [FromBody] CreateConversationRequest request)
        {
            var idComprador = GetUserId();

            var conv = await _chatService.CreateOrGetConversationAsync(
                request.IdAuto, request.IdVendedor, idComprador);

            return Ok(conv);
        }

        [HttpGet("GetMyConversations")]
        public async Task<ActionResult<IEnumerable<ChatConversationDto>>> GetMyConversations(
            [FromQuery] int skip = 0,
            [FromQuery] int take = 20)
        {
            var userId = GetUserId();
            var conversations = await _chatService.GetUserConversationsAsync(userId, skip, take);
            return Ok(conversations);
        }

        [HttpGet("GetMessages/{conversationId:guid}/messages")]
        public async Task<ActionResult<IEnumerable<ChatMessageDto>>> GetMessages(
            [FromRoute] Guid conversationId,
            [FromQuery] int skip = 0,
            [FromQuery] int take = 50)
        {
            var userId = GetUserId();
            var messages = await _chatService.GetMessagesAsync(conversationId, skip, take);
            return Ok(messages);
        }

        [HttpPost("UploadMedia")]
        public async Task<ActionResult<object>> UploadMedia(
            [FromForm] IFormFile file,
            [FromQuery] Guid? conversationId = null)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Archivo inválido.");

            var userId = GetUserId();

            var result = await _chatMediaService.UploadAsync(file, userId, conversationId);

            return Ok(new
            {
                mediaUrl = result.MediaUrl,
                thumbnailUrl = result.ThumbnailUrl,
                mediaType = result.MediaType
            });
        }
    }
}
