using  Microsoft.Extensions.Caching.Memory;
using  System.Security.Claims;
using  System.Text.RegularExpressions;

namespace AutoBarato.Comunicaciones.Api.Middleware
{
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _cache;
        private readonly RateLimitingOptions _options;
        private const string CachePrefix = "rl:";

        public RateLimitingMiddleware(RequestDelegate next, IMemoryCache cache, RateLimitingOptions options)
        {
            _next = next;
            _cache = cache;
            _options = options;
        }

        public async Task Invoke(HttpContext context)
        {
            // 0) [SkipRateLimit]
            var endpoint = context.GetEndpoint();
            if (endpoint?.Metadata?.GetMetadata<SkipRateLimitAttribute>() != null)
            {
                await _next(context);
                return;
            }

            // 1) Excluir paths globales
            var path = context.Request.Path.Value ?? string.Empty;
            if (_options.ExcludedPaths?.Any(p => Regex.IsMatch(path, p, RegexOptions.IgnoreCase)) == true)
            {
                await _next(context);
                return;
            }

            // 2) Regla por ruta (si no, default)
            var rule = _options.Rules.FirstOrDefault(r =>
                Regex.IsMatch(path, r.PathPattern, RegexOptions.IgnoreCase))
                ?? new RateLimitRule
                {
                    PermitLimit = _options.DefaultPermitLimit,
                    Window = _options.DefaultWindow
                };

            // 3) Identificador: usuario (JWT) o IP
            var userId =
                context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                context.User?.FindFirst("sub")?.Value;

            string identifier;
            if (!string.IsNullOrWhiteSpace(userId))
            {
                identifier = $"user:{userId}";
            }
            else
            {
                if (!rule.CountAnonymousByIp)
                {
                    await _next(context);
                    return;
                }
                var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown-ip";
                identifier = $"ip:{ip}";
            }

            // 4) Fixed window con MemoryCache
            var now = DateTimeOffset.UtcNow;
            var window = rule.Window;
            var windowStart = new DateTimeOffset(now.Ticks - (now.Ticks % window.Ticks), TimeSpan.Zero);
            var windowEnd = windowStart.Add(window);

            string cacheKey = $"{CachePrefix}{path}:{identifier}:{windowStart.ToUnixTimeSeconds()}";

            int current = _cache.GetOrCreate(cacheKey, entry =>
            {
                entry.AbsoluteExpiration = windowEnd;
                return 0;
            });

            if (current >= rule.PermitLimit)
            {
                var resetSeconds = (int)Math.Ceiling((windowEnd - now).TotalSeconds);
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                context.Response.Headers["Retry-After"] = resetSeconds.ToString();
                context.Response.Headers["X-RateLimit-Limit"] = rule.PermitLimit.ToString();
                context.Response.Headers["X-RateLimit-Remaining"] = "0";
                context.Response.Headers["X-RateLimit-Reset"] = windowEnd.ToUnixTimeSeconds().ToString();
                await context.Response.WriteAsync("Has excedido el límite de peticiones. Intenta más tarde.");
                return;
            }

            _cache.Set(cacheKey, current + 1, windowEnd);
            var remaining = Math.Max(0, rule.PermitLimit - (current + 1));

            context.Response.OnStarting(() =>
            {
                context.Response.Headers["X-RateLimit-Limit"] = rule.PermitLimit.ToString();
                context.Response.Headers["X-RateLimit-Remaining"] = remaining.ToString();
                context.Response.Headers["X-RateLimit-Reset"] = windowEnd.ToUnixTimeSeconds().ToString();
                return Task.CompletedTask;
            });

            await _next(context);
        }
    }
}
