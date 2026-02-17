using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoBarato.Comunicaciones.Application.DTOs.Common
{
    public class JwtSettings
    {
        public string Secret { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int ExpirationMinutes { get; set; }
        public int WebTokenExpirationMinutes { get; set; }
        public int MobileTokenExpirationMinutes { get; set; }
        public int RefreshTokenExpirationDays { get; set; }

    }
}
