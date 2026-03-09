namespace AutoBarato.Comunicaciones.Application.DTOs.Request.Archivos
{
    public class UploadFileRequest
    {
        public Func<Stream>? AbrirContenido { get; set; }
        public string? NombreArchivo { get; set; }
        public string? TipoContenido { get; set; }
        public int IdTipoArchivo { get; set; }
        public string? ObjectKey { get; set; }
        public int? IdOrden { get; set; }
    }
}