using AutoBarato.Comunicaciones.Application.Interfaces.Bitacora;
using AutoBarato.Comunicaciones.Domain.Entities.Bitacora;
using Microsoft.Extensions.Logging;

namespace AutoBarato.Comunicaciones.Infrastructure.Bitacora.Error
{
    public sealed class BitacoraErrorService : IBitacoraErrorService
    {
        private readonly BitacoraColaDeErrores _laColaDeErroresDeBitacora;
        private readonly ILogger<BitacoraErrorService> _elRegistrador;

        public BitacoraErrorService(BitacoraColaDeErrores colaDeErroresDeBitacora, ILogger<BitacoraErrorService> registrador)
        {
            _laColaDeErroresDeBitacora = colaDeErroresDeBitacora;
            _elRegistrador = registrador;
        }

        public void AgregarErrorDeBitacoraALaCola(BitacoraDeError errorDeBitacora)
        {
            // Nunca debe tumbar request
            try
            {
                var elErrorFueAgregado = _laColaDeErroresDeBitacora.IntentarAgregarError(errorDeBitacora);
                _elRegistrador.LogInformation("🧾 Bitácora ENQUEUE ok={Ok} TraceId={TraceId} Path={Path}",
                    elErrorFueAgregado, errorDeBitacora.TraceId, errorDeBitacora.RequestPath);
                if (!elErrorFueAgregado)
                {
                    _elRegistrador.LogWarning("BitacoraQueue llena. Se descartó log de error. TraceId={TraceId}", errorDeBitacora.TraceId);
                }
            }
            catch (Exception laExcepcion)
            {
                _elRegistrador.LogError(laExcepcion, "Fallo en AgregarErrorDeBitacoraALaCola (bitácora). Se ignora para no afectar request.");
            }
        }
    }
}
