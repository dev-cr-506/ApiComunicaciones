using AutoBarato.Comunicaciones.Domain.Entities.Bitacora;

namespace AutoBarato.Comunicaciones.Application.Interfaces.Bitacora
{
    public interface IBitacoraEventosService
    {
        void AgregarEventoDeBitacoraALaCola(BitacoraEvento evento);
    }
}
