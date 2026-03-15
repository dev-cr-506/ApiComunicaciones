
using AutoBarato.Comunicaciones.Application.DTOs.Response.Comunicaciones;
using Microsoft.AspNetCore.Http;

namespace AutoBarato.Comunicaciones.Application.Interfaces
{
    public interface IChatMediaService
    {
        Task<ChatMediaResponse> SubirMedioAsync( IFormFile archivo,  int idUsuario, Guid? idConversacion = null);
    }
}
