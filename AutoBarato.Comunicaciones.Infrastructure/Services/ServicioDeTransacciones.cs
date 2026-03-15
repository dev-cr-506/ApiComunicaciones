using AutoBarato.Comunicaciones.Infrastructure.DataAccess;
using AutoBarato.Comunicaciones.Application.Interfaces;

namespace AutoBarato.Comunicaciones.Infrastructure.Services
{
    public class ServicioDeTransacciones : IServicioDeTransacciones
    {
        private readonly ComunicacionesDbContext _elContextoDeBaseDeDatos;

        public ServicioDeTransacciones(ComunicacionesDbContext contextoDeBaseDeDatos)
        {
            _elContextoDeBaseDeDatos = contextoDeBaseDeDatos;
        }

        public async Task EjecutarEnTransaccionAsync(Func<Task> accion)
        {
            await using var laTransaccion = await _elContextoDeBaseDeDatos.Database.BeginTransactionAsync();

            try
            {
                await accion();
                await laTransaccion.CommitAsync();
            }
            catch
            {
                await laTransaccion.RollbackAsync();
                throw;
            }
        }

        public async Task<T> EjecutarEnTransaccionAsync<T>(Func<Task<T>> accion)
        {
            await using var laTransaccion = await _elContextoDeBaseDeDatos.Database.BeginTransactionAsync();

            try
            {
                var elResultado = await accion();
                await laTransaccion.CommitAsync();
                return elResultado;
            }
            catch
            {
                await laTransaccion.RollbackAsync();
                throw;
            }
        }
    }

}
