using AutoBarato.Comunicaciones.Domain.Entities.Bitacora;
using System.Threading.Channels;

namespace AutoBarato.Comunicaciones.Infrastructure.Bitacora.Error
{
    public sealed class BitacoraColaDeErrores
    {
        private readonly Channel<BitacoraDeError> _elCanalDeErroresDeBitacora;

        public BitacoraColaDeErrores(int capacidad)
        {
            var lasOpcionesDelCanalLimitado = new BoundedChannelOptions(capacidad)
            {
                SingleReader = true,
                SingleWriter = false,
                FullMode = BoundedChannelFullMode.DropOldest
            };

            _elCanalDeErroresDeBitacora = Channel.CreateBounded<BitacoraDeError>(lasOpcionesDelCanalLimitado);
        }

        public bool IntentarAgregarError(BitacoraDeError errorDeBitacora) => _elCanalDeErroresDeBitacora.Writer.TryWrite(errorDeBitacora);

        // ✅ Para drenar rápido batch
        public bool IntentarLeerError(out BitacoraDeError? errorDeBitacora) => _elCanalDeErroresDeBitacora.Reader.TryRead(out errorDeBitacora);

        // ✅ Para esperar un item (bloqueante)
        public ValueTask<BitacoraDeError> LeerErrorAsync(CancellationToken tokenCancelacion) => _elCanalDeErroresDeBitacora.Reader.ReadAsync(tokenCancelacion);
    }
}