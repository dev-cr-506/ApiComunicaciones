using  System;
using  System.Collections.Generic;
using  System.Linq;
using  System.Text;
using  System.Threading.Tasks;

namespace AutoBarato.Comunicaciones.Domain.Configuration
{
    public class StorageSettings
    {
        public Dictionary<string, BucketConfig> Buckets { get; set; } = new();
        public string DefaultBucket { get; set; } = string.Empty;
    }

    public class BucketConfig
    {
        public string BasePath { get; set; } = string.Empty;
        public string Folder { get; set; } = string.Empty;
    }
}
