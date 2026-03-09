using AutoBarato.Comunicaciones.Domain.Entities.Bitacora;

namespace AutoBarato.Comunicaciones.Domain.Interfaces.Bitacora
{
    public interface IBitacoraEventoRepository
    {
        Task InsertarEventoAsync(BitacoraEvento eventoDeBitacora, CancellationToken tokenCancelacion = default);
        Task InsertarLoteDeEventosAsync(IReadOnlyCollection<BitacoraEvento> loteDeEventosDeBitacora, CancellationToken tokenCancelacion = default);
    }
}
