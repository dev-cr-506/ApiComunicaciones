using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;

namespace AutoBarato.Comunicaciones.Api.Utils
{
    public static class Utils
    {
        public static int ObtenerUsuarioDesdeToken(string? rawAuthorizationHeader)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(rawAuthorizationHeader))
                    return 0;

                var authHeader = AuthenticationHeaderValue.Parse(rawAuthorizationHeader);

                // authHeader.Scheme  => "Bearer"
                // authHeader.Parameter => el JWT puro
                var token = authHeader.Parameter;
                if (string.IsNullOrWhiteSpace(token))
                    return 0;

                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                var usuarioClaim = jwtToken.Claims
                    .FirstOrDefault(c => c.Type == "IdUsuario")
                    ?.Value;

                if (string.IsNullOrWhiteSpace(usuarioClaim))
                    return 0;

                return Convert.ToInt32(usuarioClaim);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al procesar el token: {ex.Message}");
                return 0;
            }
        }
    }
}
