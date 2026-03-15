using AutoBarato.Comunicaciones.Api.Extensions;
using AutoBarato.Comunicaciones.Api.Models.Common;
using AutoBarato.Comunicaciones.Application.DTOs.Response.Comunicaciones;
using AutoBarato.Comunicaciones.Application.Interfaces;
using AutoBarato.Comunicaciones.Application.Models.Request.Comunicaciones;
using Microsoft.AspNetCore.Mvc;
using System.Net;

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
            return 1;
        }

       

        [HttpPost("CreateOrGetConversation")]
        public async Task<ActionResult<ChatConversacionResponse>> CrearOObtenerConversacion(
            [FromBody] SolicitudDeCrearConversacionRequest solicitudDeConversacion)
        {
            //var elIdComprador = ObtenerIdUsuario();

            var idUsuarioAutenticado = User.ObtenerIdDeUsuarioAutenticado();

            if (idUsuarioAutenticado <= 0)
            {
                return Unauthorized(Response<ChatConversacionResponse?>.ErrorResponse(
                    HttpStatusCode.Unauthorized,
                    "No se pudo obtener el usuario autenticado.",
                    new List<ErrorResponse>
                    {
                        new ErrorResponse { ErrorCode = 401, Message = "Usuario inválido en el token." }
                    }
                ));
            }

            var laConversacion = await _elServicioDeChat.CrearOObtenerConversacionAsync(
                solicitudDeConversacion.IdAuto, solicitudDeConversacion.IdVendedor, idUsuarioAutenticado);

            return Ok(laConversacion);
        }

        [HttpGet("GetMyConversations")]
        public async Task<ActionResult<IEnumerable<ChatConversacionResponse>>> ObtenerMisConversaciones(
            [FromQuery] int omitir = 0,
            [FromQuery] int tomar = 20)
        {
            var elIdUsuario = ObtenerIdUsuario();
            var lasConversaciones = await _elServicioDeChat.ObtenerMisConversacionesAsync(elIdUsuario, omitir, tomar);
            return Ok(lasConversaciones);
        }

        [HttpGet("GetMessages/{idConversacion:guid}/messages")]
        public async Task<ActionResult<IEnumerable<ChatMensajeResponse>>> ObtenerMensajes(
            [FromRoute] Guid idConversacion,
            [FromQuery] int omitir = 0,
            [FromQuery] int tomar = 50)
        {
            var elIdUsuario = ObtenerIdUsuario();
            var losMensajes = await _elServicioDeChat.ObtenerMensajesAsync(idConversacion, omitir, tomar);
            return Ok(losMensajes);
        }

        [HttpPost("UploadMedia")]
        public async Task<ActionResult<object>> SubirMedio(
            IFormFile archivo,
            [FromQuery] Guid? idConversacion = null)
        {
            if (archivo == null || archivo.Length == 0)
            {
                return BadRequest("Archivo inválido.");
            }

            var elIdUsuario = ObtenerIdUsuario();

            var elResultadoDeCarga = await _elServicioDeMediosDeChat.SubirMedioAsync(archivo, elIdUsuario, idConversacion);

            return Ok(new
            {
                mediaUrl = elResultadoDeCarga.MediaUrl,
                thumbnailUrl = elResultadoDeCarga.ThumbnailUrl,
                mediaType = elResultadoDeCarga.MediaType
            });
        }
    }
}