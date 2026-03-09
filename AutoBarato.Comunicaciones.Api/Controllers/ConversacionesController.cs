using AutoBarato.Comunicaciones.Api.Models.Request.Comunicaciones;
using AutoBarato.Comunicaciones.Application.DTOs.Response.Comunicaciones;
using AutoBarato.Comunicaciones.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AutoBarato.Comunicaciones.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConversacionesController : ControllerBase
    {
        private readonly IChatService _elServicioDeChat;
        private readonly IChatMediaService _elServicioDeMediosDeChat;

        public ConversacionesController(IChatService servicioDeChat, IChatMediaService servicioDeMediosDeChat)
        {
            _elServicioDeChat = servicioDeChat;
            _elServicioDeMediosDeChat = servicioDeMediosDeChat;
        }

        [Microsoft.AspNetCore.Mvc.NonAction]
        protected int ObtenerIdUsuario()
        {
            var elIdUsuarioDelClaim = User.FindFirst("IdUsuario")?.Value;
            return Utils.Utils.ObtenerUsuarioDesdeToken(Request.Headers.Authorization);
        }

       

        [HttpPost("CreateOrGetConversation")]
        public async Task<ActionResult<ChatConversacionResponse>> CrearOObtenerConversacionAsync(
            [FromBody] SolicitudDeCrearConversacion solicitudDeConversacion)
        {
            var elIdComprador = ObtenerIdUsuario();

            var laConversacion = await _elServicioDeChat.CreateOrGetConversationAsync(
                solicitudDeConversacion.IdAuto, solicitudDeConversacion.IdVendedor, elIdComprador);

            return Ok(laConversacion);
        }

        [HttpGet("GetMyConversations")]
        public async Task<ActionResult<IEnumerable<ChatConversacionResponse>>> ObtenerMisConversacionesAsync(
            [FromQuery] int omitir = 0,
            [FromQuery] int tomar = 20)
        {
            var elIdUsuario = ObtenerIdUsuario();
            var lasConversaciones = await _elServicioDeChat.GetUserConversationsAsync(elIdUsuario, omitir, tomar);
            return Ok(lasConversaciones);
        }

        [HttpGet("GetMessages/{idConversacion:guid}/messages")]
        public async Task<ActionResult<IEnumerable<ChatMensajeResponse>>> ObtenerMensajesAsync(
            [FromRoute] Guid idConversacion,
            [FromQuery] int omitir = 0,
            [FromQuery] int tomar = 50)
        {
            var elIdUsuario = ObtenerIdUsuario();
            var losMensajes = await _elServicioDeChat.GetMessagesAsync(idConversacion, omitir, tomar);
            return Ok(losMensajes);
        }

        [HttpPost("UploadMedia")]
        public async Task<ActionResult<object>> SubirMedioAsync(
            IFormFile archivo,
            [FromQuery] Guid? idConversacion = null)
        {
            if (archivo == null || archivo.Length == 0)
            {
                return BadRequest("Archivo inválido.");
            }

            var elIdUsuario = ObtenerIdUsuario();

            var elResultadoDeCarga = await _elServicioDeMediosDeChat.UploadAsync(archivo, elIdUsuario, idConversacion);

            return Ok(new
            {
                mediaUrl = elResultadoDeCarga.MediaUrl,
                thumbnailUrl = elResultadoDeCarga.ThumbnailUrl,
                mediaType = elResultadoDeCarga.MediaType
            });
        }
    }
}