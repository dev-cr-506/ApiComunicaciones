using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoBarato.Comunicaciones.Application.DTOs
{
    public class ChatMessageDto
    {
        public Guid Id { get; set; }
        public Guid ConversationId { get; set; }
        public int RemitenteId { get; set; }
        public string? Texto { get; set; }
        public string TipoMensaje { get; set; } = "TEXT";
        public string? MediaUrl { get; set; }
        public string? MediaThumbnailUrl { get; set; }
        public DateTime FechaEnvio { get; set; }
        public bool EsEditado { get; set; }
        public DateTime? FechaEdicion { get; set; }
        public bool Leido { get; set; }
        public bool MediaDisponible { get; set; }
    }
}
