using AutoBarato.Comunicaciones.Application.DTOs;
using AutoBarato.Comunicaciones.Domain.Entities;
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
