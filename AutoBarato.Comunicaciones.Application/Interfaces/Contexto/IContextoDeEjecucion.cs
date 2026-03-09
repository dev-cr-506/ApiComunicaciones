namespace AutoBarato.Comunicaciones.Application.Interfaces.Contexto
{
    public interface IContextoDeEjecucion
    {
        string? ObtenerTraceId();
        string? ObtenerIdDeCorrelacion();
    }
}
