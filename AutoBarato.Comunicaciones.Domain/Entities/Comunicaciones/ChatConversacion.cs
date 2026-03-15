

namespace AutoBarato.Comunicaciones.Domain.Entities.Comunicaciones
{
    public class ChatConversacion
    {
        public Guid Id { get; set; }
        public int IdAuto { get; set; }
        public int IdVendedor { get; set; }
        public int IdComprador { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string Estado { get; set; } 
        public string? UltimoMensaje { get; set; }
        public string? NombreVendedor { get; set; }
        public string? MarcaModelo { get; set; }
        public DateTime UltimaActividad { get; set; }


    }
}
