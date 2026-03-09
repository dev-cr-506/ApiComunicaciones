using AutoBarato.Comunicaciones.Infrastructure.DataAccess;
using AutoBarato.Comunicaciones.Application.Interfaces;
using AutoBarato.Comunicaciones.Infrastructure.DataAccess;

namespace AutoBarato.Comunicaciones.Infrastructure.Services
{
    public class ServicioDeTransacciones : IServicioDeTransacciones
    {
        private readonly ComunicacionesDbContext _dbContext;

        public ServicioDeTransacciones(ComunicacionesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task ExecuteInTransactionAsync(Func<Task> action)
        {
            await using  var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                await action();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> action)
        {
            await using  var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                var result = await action();
                await transaction.CommitAsync();
                return result;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }

}
