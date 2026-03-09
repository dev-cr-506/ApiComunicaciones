using  System;
using  System.Collections.Generic;
using  System.Linq;
using  System.Text;
using  System.Threading.Tasks;

namespace AutoBarato.Comunicaciones.Domain.Configuration
{
    public class ConfiguracionDeBucketsDeAlmacenamiento
    {
        public Dictionary<string, ConfiguracionDeBucket> Buckets { get; set; } = new();
        public string DefaultBucket { get; set; } = string.Empty;
    }


}
