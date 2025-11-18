using AutoBarato.Comunicaciones.Application.DTOs;
using AutoBarato.Comunicaciones.Domain.Entities;
using  AutoMapper;

namespace AutoBarato.Comunicaciones.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ChatConversation, ChatConversationDto>();
            CreateMap<ChatMessage, ChatMessageDto>();
        }
    }
}
