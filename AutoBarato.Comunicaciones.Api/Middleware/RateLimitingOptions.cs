using  System;

namespace AutoBarato.Comunicaciones.Api.Middleware
{
    public class RateLimitRule
    {
        public string PathPattern { get; set; } = ".*";                // Regex de ruta
        public int PermitLimit { get; set; } = 60;                      // req por ventana
        public TimeSpan Window { get; set; } = TimeSpan.FromMinutes(1); // tamaño de ventana
        public bool CountAnonymousByIp { get; set; } = true;            // anónimos por IP
    }

    public class RateLimitingOptions
    {
        public int DefaultPermitLimit { get; set; } = 60;
        public TimeSpan DefaultWindow { get; set; } = TimeSpan.FromMinutes(1);
        public string[] ExcludedPaths { get; set; } = new[] { "^/swagger", "^/health", "^/hc" };
        public List<RateLimitRule> Rules { get; } = new();
    }
}
