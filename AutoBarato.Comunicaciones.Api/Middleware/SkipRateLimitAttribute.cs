using  System;

namespace AutoBarato.Comunicaciones.Api.Middleware
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public sealed class SkipRateLimitAttribute : Attribute { }
}
