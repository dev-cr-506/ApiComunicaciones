using AutoBarato.Comunicaciones.Application.DTOs.Response.Comunicaciones;
using AutoBarato.Comunicaciones.Domain.Entities.Comunicaciones;
using AutoMapper;

namespace AutoBarato.Comunicaciones.Application.Mapping
{
    public class ComunicacionesMappingProfile : Profile
    {
        public ComunicacionesMappingProfile()
        {
            CreateMap<ChatConversacion, ChatConversacionResponse>();
            CreateMap<ChatMensaje, ChatMensajeResponse>();
        }
    }
}
