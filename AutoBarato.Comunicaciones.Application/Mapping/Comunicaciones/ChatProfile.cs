using AutoBarato.Comunicaciones.Application.DTOs.Response.Comunicaciones;
using AutoBarato.Comunicaciones.Domain.Entities.Comunicaciones;
using AutoMapper;

namespace AutoBarato.Comunicaciones.Application.Mapping.Comunicaciones
{
    public class ChatProfile : Profile
    {
        public ChatProfile()
        {
            CreateMap<ChatConversacion, ChatConversacionResponse>();
            CreateMap<ChatMensaje, ChatMensajeResponse>();
        }
    }
}
