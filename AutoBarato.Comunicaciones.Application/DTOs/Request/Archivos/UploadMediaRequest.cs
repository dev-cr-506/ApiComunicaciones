namespace AutoBarato.Comunicaciones.Application.DTOs.Request.Archivos
{
    public class UploadMediaRequest
    {
        public List<UploadFileRequest>? Files { get; set; }
        public string DestinoFolderBucket { get; set; }
    }
}
