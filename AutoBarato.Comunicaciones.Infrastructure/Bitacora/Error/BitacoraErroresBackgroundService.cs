using AutoBarato.Comunicaciones.Domain.Entities.Bitacora;
using AutoBarato.Comunicaciones.Domain.Interfaces.Bitacora;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AutoBarato.Comunicaciones.Infrastructure.Bitacora.Error
{
    public sealed class BitacoraErroresBackgroundService : BackgroundService
    {
        private readonly BitacoraColaDeErrores _laColaDeBitacora;
        private readonly IServiceProvider _elProveedorDeServicios;
        private readonly ILogger<BitacoraErroresBackgroundService> _elRegistrador;
        private readonly BitacoraErrorOptions _lasOpcionesDeBitacora;

        public BitacoraErroresBackgroundService(
            BitacoraColaDeErrores colaDeBitacora,
            IServiceProvider proveedorDeServicios,
            IOptions<BitacoraErrorOptions> opciones,
            ILogger<BitacoraErroresBackgroundService> registrador)
        {
            _laColaDeBitacora = colaDeBitacora;
            _elProveedorDeServicios = proveedorDeServicios;
            _elRegistrador = registrador;
            _lasOpcionesDeBitacora = opciones.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!_lasOpcionesDeBitacora.Enabled)
            {
                _elRegistrador.LogInformation("BitacoraWriter deshabilitado por configuración.");
                return;
            }

            var elBufferDeErroresDeBitacora = new List<BitacoraDeError>(_lasOpcionesDeBitacora.MaxBatchSize);
            using var elTemporizador = new PeriodicTimer(TimeSpan.FromMilliseconds(_lasOpcionesDeBitacora.FlushIntervalMs));

            try
            {
                while (await elTemporizador.WaitForNextTickAsync(stoppingToken))
                {
                    // 1) Drenar toda la cola disponible ahora (sin bloquear)
                    while (_laColaDeBitacora.IntentarLeerError(out var errorDeBitacora) && errorDeBitacora != null)
                    {
                        elBufferDeErroresDeBitacora.Add(errorDeBitacora);

                        // Si llegamos al batch max, flush inmediato
                        if (elBufferDeErroresDeBitacora.Count >= _lasOpcionesDeBitacora.MaxBatchSize)
                        {
                            await GuardarLoteAsync(elBufferDeErroresDeBitacora, stoppingToken);
                            elBufferDeErroresDeBitacora.Clear();
                        }
                    }

                    // 2) Flush por tiempo (si hay algo pendiente)
                    if (elBufferDeErroresDeBitacora.Count > 0)
                    {
                        await GuardarLoteAsync(elBufferDeErroresDeBitacora, stoppingToken);
                        elBufferDeErroresDeBitacora.Clear();
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // normal al apagar
            }
            finally
            {
                // Flush final: drenar lo que quede y persistir
                while (_laColaDeBitacora.IntentarLeerError(out var errorDeBitacora) && errorDeBitacora != null)
                    elBufferDeErroresDeBitacora.Add(errorDeBitacora);

                if (elBufferDeErroresDeBitacora.Count > 0)
                {
                    await GuardarLoteAsync(elBufferDeErroresDeBitacora, CancellationToken.None);
                    elBufferDeErroresDeBitacora.Clear();
                }
            }
        }

        private async Task GuardarLoteAsync(List<BitacoraDeError> erroresDeBitacoraEnBuffer, CancellationToken tokenCancelacion)
        {
            if (erroresDeBitacoraEnBuffer.Count == 0)
            {
                return;
            }

            try
            {
                using var elAmbito = _elProveedorDeServicios.CreateScope();
                var elRepositorioDeBitacoraDeError = elAmbito.ServiceProvider.GetRequiredService<IBitacoraDeErrorRepository>();

                await elRepositorioDeBitacoraDeError.InsertarLoteDeErroresAsync(erroresDeBitacoraEnBuffer, tokenCancelacion);
            }
            catch (Exception laExcepcion)
            {
                // No tumba el request; solo se pierden logs si BD está caída
                _elRegistrador.LogError(laExcepcion, "Error persistiendo bitácora (batch). Se perderán {Count} logs.", erroresDeBitacoraEnBuffer.Count);
            }
        }
    }
}
