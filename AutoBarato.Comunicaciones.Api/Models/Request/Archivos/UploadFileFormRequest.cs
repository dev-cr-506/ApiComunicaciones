namespace AutoBarato.Comunicaciones.Api.Models.Request.Archivos
{
    public class UploadFileFormRequest
    {
        public IFormFile? File { get; set; }
        public int IdTipoArchivo { get; set; }
        public string? ObjectKey { get; set; }
        public int? IdOrden { get; set; }
    }
}
