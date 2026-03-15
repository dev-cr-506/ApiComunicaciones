using AutoBarato.Comunicaciones.Api.Models.Request.Archivos;
using AutoBarato.Comunicaciones.Application.DTOs.Request.Archivos;

namespace AutoBarato.Comunicaciones.Api.Mappers
{
    public static class ArchivoRequestMapper
    {
        public static List<UploadFileRequest> ConvertirArchivos(List<UploadFileFormRequest>? archivosDelFormulario)
        {
            var losArchivosConvertidos = new List<UploadFileRequest>();

            if (archivosDelFormulario == null || archivosDelFormulario.Count == 0)
            {
                return losArchivosConvertidos;
            }

            foreach (var elArchivoDelFormulario in archivosDelFormulario)
            {
                losArchivosConvertidos.Add(new UploadFileRequest
                {
                    AbrirContenido = elArchivoDelFormulario.File != null
                        ? () => elArchivoDelFormulario.File.OpenReadStream()
                        : null,
                    NombreArchivo = elArchivoDelFormulario.File?.FileName,
                    TipoContenido = elArchivoDelFormulario.File?.ContentType,
                    IdTipoArchivo = elArchivoDelFormulario.IdTipoArchivo,
                    ObjectKey = elArchivoDelFormulario.ObjectKey,
                    IdOrden = elArchivoDelFormulario.IdOrden
                });
            }

            return losArchivosConvertidos;
        }
    }
}
