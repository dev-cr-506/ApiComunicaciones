using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoBarato.Comunicaciones.Application.DTOs
{
    public class UploadFilesResponse
    {
        public string FileUrl { get; set; }
        public int IdTipoArchivo { get; set; }      // <-- int, no string
        public string ObjectKey { get; set; }       // <-- agrega si lo necesitas
        public string Message { get; set; }
        public bool Success { get; set; }
    }
}
