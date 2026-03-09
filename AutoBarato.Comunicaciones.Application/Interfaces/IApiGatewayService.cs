using AutoBarato.Comunicaciones.Application.DTOs.Request.Archivos;
using AutoBarato.Comunicaciones.Application.DTOs.Response.Archivos;

namespace AutoBarato.Comunicaciones.Application.Interfaces
{
    public interface IApiGatewayService
    {
        Task<List<UploadFilesResponse>> ProcesarArchivosAutoConFailoverAsync(UploadMediaRequest request);
        Task<List<DeleteFilesResponse>> EliminarArchivosS3Async(DeleteFilesRequest request);
    }
}
