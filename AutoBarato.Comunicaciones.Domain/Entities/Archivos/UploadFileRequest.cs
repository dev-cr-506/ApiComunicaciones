using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoBarato.Comunicaciones.Domain.Entities.Archivos
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
