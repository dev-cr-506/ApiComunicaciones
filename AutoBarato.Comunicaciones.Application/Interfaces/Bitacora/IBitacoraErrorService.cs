using AutoBarato.Comunicaciones.Domain.Entities.Bitacora;

namespace AutoBarato.Comunicaciones.Application.Interfaces.Bitacora
{
    public interface IBitacoraErrorService
    {
        void AgregarErrorDeBitacoraALaCola(BitacoraDeError errorDeBitacora);
    }
}
