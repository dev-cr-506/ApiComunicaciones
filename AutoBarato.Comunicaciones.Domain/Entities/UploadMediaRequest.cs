using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoBarato.Comunicaciones.Domain.Entities
{
    public class UploadMediaRequest
    {
        public List<UploadFileRequest>? Files { get; set; }
        public string DestinoFolderBucket { get; set; }
    }
}
