using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoBarato.Comunicaciones.Application.DTOs
{
    public class ChatConversationDto
    {
        public Guid Id { get; set; }
        public int IdAuto { get; set; }
        public int IdVendedor { get; set; }
        public int IdComprador { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string Estado { get; set; } = "ACTIVA";
        public string? UltimoMensaje { get; set; }
        public string? NombreVendedor { get; set; }
        public string? MarcaModelo { get; set; }
        public DateTime UltimaActividad { get; set; }
    }
}
