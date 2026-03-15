

namespace AutoBarato.Comunicaciones.Application.Interfaces
{
    public interface IServicioDeTransacciones
    {
        Task EjecutarEnTransaccionAsync(Func<Task> accion);
        Task<T> EjecutarEnTransaccionAsync<T>(Func<Task<T>> accion);
    }

}
