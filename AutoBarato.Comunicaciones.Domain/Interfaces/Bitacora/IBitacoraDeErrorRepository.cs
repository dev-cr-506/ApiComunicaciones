using AutoBarato.Comunicaciones.Domain.Entities.Bitacora;

namespace AutoBarato.Comunicaciones.Domain.Interfaces.Bitacora
{
    public interface IBitacoraDeErrorRepository
    {
        Task InsertarErrorEnBitacoraAsync(BitacoraDeError errorDeBitacora, CancellationToken tokenCancelacion = default);
        Task InsertarLoteDeErroresAsync(IReadOnlyCollection<BitacoraDeError> loteDeErroresDeBitacora, CancellationToken tokenCancelacion = default);
    }
}
