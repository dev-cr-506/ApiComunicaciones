using System.Security.Claims;

namespace AutoBarato.Comunicaciones.Api.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static int ObtenerIdDeUsuarioAutenticado(this ClaimsPrincipal usuario)
        {
            var elValorDelIdDeUsuario =
                usuario.FindFirst("IdUsuario")?.Value ??
                usuario.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                usuario.FindFirst("sub")?.Value;

            return int.TryParse(elValorDelIdDeUsuario, out var elIdDeUsuario) ? elIdDeUsuario : 0;
        }
    }
}
