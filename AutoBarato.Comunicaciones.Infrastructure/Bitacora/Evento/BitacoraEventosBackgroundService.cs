using AutoBarato.Comunicaciones.Domain.Entities.Bitacora;
using AutoBarato.Comunicaciones.Domain.Interfaces.Bitacora;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AutoBarato.Comunicaciones.Infrastructure.Bitacora.Evento
{
    public sealed class BitacoraEventosBackgroundService : BackgroundService
    {
        private readonly BitacoraColaDeEventos _laColaDeEventosDeBitacora;
        private readonly IServiceProvider _elProveedorDeServicios;
        private readonly ILogger<BitacoraEventosBackgroundService> _elRegistrador;
        private readonly BitacoraEventosOptions _lasOpcionesDeEventosDeBitacora;

        public BitacoraEventosBackgroundService(
            BitacoraColaDeEventos colaDeEventosDeBitacora,
            IServiceProvider proveedorDeServicios,
            IOptions<BitacoraEventosOptions> opciones,
            ILogger<BitacoraEventosBackgroundService> registrador)
        {
            _laColaDeEventosDeBitacora = colaDeEventosDeBitacora;
            _elProveedorDeServicios = proveedorDeServicios;
            _elRegistrador = registrador;
            _lasOpcionesDeEventosDeBitacora = opciones.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            if (!_lasOpcionesDeEventosDeBitacora.Enabled)
            {
                return;
            }

            var elBufferDeEventosDeBitacora = new List<BitacoraEvento>(_lasOpcionesDeEventosDeBitacora.MaxBatchSize);
            using var elTemporizador = new PeriodicTimer(TimeSpan.FromMilliseconds(_lasOpcionesDeEventosDeBitacora.FlushIntervalMs));

            try
            {
                while (await elTemporizador.WaitForNextTickAsync(stoppingToken))
                {
                    // 1) Drenar sin bloquear
                    while (_laColaDeEventosDeBitacora.IntentarLeerEvento(out var eventoDeBitacora) && eventoDeBitacora != null)
                    {
                        elBufferDeEventosDeBitacora.Add(eventoDeBitacora);

                        if (elBufferDeEventosDeBitacora.Count >= _lasOpcionesDeEventosDeBitacora.MaxBatchSize)
                        {
                            await GuardarLoteAsync(elBufferDeEventosDeBitacora, stoppingToken);
                            elBufferDeEventosDeBitacora.Clear();
                        }
                    }

                    // 2) Flush por tiempo si hay algo
                    if (elBufferDeEventosDeBitacora.Count > 0)
                    {
                        await GuardarLoteAsync(elBufferDeEventosDeBitacora, stoppingToken);
                        elBufferDeEventosDeBitacora.Clear();
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // normal en shutdown
            }
            finally
            {
                // Flush final
                while (_laColaDeEventosDeBitacora.IntentarLeerEvento(out var eventoDeBitacora) && eventoDeBitacora != null)
                {
                    elBufferDeEventosDeBitacora.Add(eventoDeBitacora);
                }

                if (elBufferDeEventosDeBitacora.Count > 0)
                {
                    await GuardarLoteAsync(elBufferDeEventosDeBitacora, CancellationToken.None);
                    elBufferDeEventosDeBitacora.Clear();
                }
            }
        }

        private async Task GuardarLoteAsync(List<BitacoraEvento> eventosDeBitacoraEnBuffer, CancellationToken tokenCancelacion)
        {
            if (eventosDeBitacoraEnBuffer.Count == 0)
            {
                return;
            }

            try
            {
                using var elAmbito = _elProveedorDeServicios.CreateScope();
                var elRepositorioDeBitacoraEvento = elAmbito.ServiceProvider.GetRequiredService<IBitacoraEventoRepository>();

                await elRepositorioDeBitacoraEvento.InsertarLoteDeEventosAsync(eventosDeBitacoraEnBuffer, tokenCancelacion);
            }
            catch (Exception laExcepcion)
            {
                _elRegistrador.LogError(laExcepcion, "Error persistiendo bitácora de eventos (batch). Se perderán {Count} eventos.", eventosDeBitacoraEnBuffer.Count);
            }
        }
    }
}
