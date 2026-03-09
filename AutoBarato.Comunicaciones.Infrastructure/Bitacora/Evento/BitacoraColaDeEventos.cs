using AutoBarato.Comunicaciones.Domain.Entities.Bitacora;
using System.Threading.Channels;

namespace AutoBarato.Comunicaciones.Infrastructure.Bitacora.Evento
{
    public sealed class BitacoraColaDeEventos
    {
        private readonly Channel<BitacoraEvento> _elCanalDeEventosDeBitacora;

        public BitacoraColaDeEventos(int capacidad)
        {
            var lasOpcionesDelCanalLimitado = new BoundedChannelOptions(capacidad)
            {
                SingleReader = true,
                SingleWriter = false,
                FullMode = BoundedChannelFullMode.DropOldest
            };

            _elCanalDeEventosDeBitacora = Channel.CreateBounded<BitacoraEvento>(lasOpcionesDelCanalLimitado);
        }

        public bool IntentarAgregarEvento(BitacoraEvento eventoDeBitacora) => _elCanalDeEventosDeBitacora.Writer.TryWrite(eventoDeBitacora);

        public bool IntentarLeerEvento(out BitacoraEvento? eventoDeBitacora) => _elCanalDeEventosDeBitacora.Reader.TryRead(out eventoDeBitacora);
    }
}
