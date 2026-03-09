using AutoBarato.Comunicaciones.Application.DTOs.Response.Archivos;
using AutoBarato.Comunicaciones.Domain.Entities.Archivos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoBarato.Comunicaciones.Application.Interfaces
{
    public interface IApiFilesService
    {
        Task<List<UploadFilesResponse>> ProcesarArchivosAutoConFailover(UploadMediaRequest request);

    }
}
