using  Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using  Microsoft.IdentityModel.Tokens;

namespace AutoBarato.Comunicaciones.Api.Middleware
{
    /// <summary>
    /// Si el request trae Authorization: Bearer ...:
    ///  - Intenta autenticar con el esquema JWT
    ///  - Si falla por expiración/invalidez → 401
    /// Si NO trae Authorization, deja pasar como anónimo.
    /// </summary>
    public class StrictExpiryOnlyMiddleware : IMiddleware
    {
        public StrictExpiryOnlyMiddleware() { }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            var hasBearer = !string.IsNullOrWhiteSpace(authHeader) &&
                            authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase);

            if (hasBearer)
            {
                var auth = context.RequestServices.GetRequiredService<IAuthenticationService>();
                var result = await auth.AuthenticateAsync(context, JwtBearerDefaults.AuthenticationScheme);

                // Importante: ValidateLifetime + ClockSkew=0 ya está configurado en Program.cs
                if (!result.Succeeded)
                {
                    // Si fue por expiración u otra causa, devolvemos 401
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Token inválido o expirado.");
                    return;
                }

                // Si quieres, puedes setear explícitamente el Principal:
                if (result.Principal != null)
                    context.User = result.Principal;
            }

            await next(context);
        }
    }
}
