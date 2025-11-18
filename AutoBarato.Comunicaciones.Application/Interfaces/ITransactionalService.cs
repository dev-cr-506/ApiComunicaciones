using  System;
using  System.Collections.Generic;
using  System.Linq;
using  System.Text;
using  System.Threading.Tasks;

namespace AutoBarato.Comunicaciones.Application.Interfaces
{
    public interface ITransactionalService
    {
        Task ExecuteInTransactionAsync(Func<Task> action);
        Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> action);
    }

}
