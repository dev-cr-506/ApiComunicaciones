namespace AutoBarato.Comunicaciones.Application.DTOs.Response.Archivos
{
    public class UploadFilesResponse
    {
        public string FileUrl { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
        public string ObjectKey { get; set; }
        public int IdTipoArchivo { get; set; }
        public int? IdColorAuto { get; set; }
        public int? IdOrden { get; set; }
    }
}
