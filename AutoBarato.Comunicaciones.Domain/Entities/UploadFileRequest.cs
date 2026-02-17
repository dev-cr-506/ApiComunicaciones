using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoBarato.Comunicaciones.Domain.Entities
{
    public class UploadFileRequest
    {
        [FromForm] public IFormFile? File { get; set; }
        [FromForm] public int IdTipoArchivo { get; set; } = 1;

    }
}
