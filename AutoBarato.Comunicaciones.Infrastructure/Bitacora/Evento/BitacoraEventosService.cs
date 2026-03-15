using AutoBarato.Comunicaciones.Application.Interfaces.Bitacora;
using AutoBarato.Comunicaciones.Domain.Entities.Bitacora;

using Microsoft.Extensions.Logging;

namespace AutoBarato.Comunicaciones.Infrastructure.Bitacora.Evento
{
    public sealed class BitacoraEventosService : IBitacoraEventosService
    {
        private readonly BitacoraColaDeEventos _laColaDeEventosDeBitacora;
        private readonly ILogger<BitacoraEventosService> _elRegistrador;

        public BitacoraEventosService(BitacoraColaDeEventos colaDeEventosDeBitacora, ILogger<BitacoraEventosService> registrador)
        {
            _laColaDeEventosDeBitacora = colaDeEventosDeBitacora;
            _elRegistrador = registrador;
        }

        public void AgregarEventoDeBitacoraALaCola(BitacoraEvento evento)
        {
            try
            {
                var elEventoDeBitacoraLimpio = LimpiarEventoDeBitacora(evento);

                var elEventoFueAgregado = _laColaDeEventosDeBitacora.IntentarAgregarEvento(elEventoDeBitacoraLimpio);
                if (!elEventoFueAgregado)
                {
                    _elRegistrador.LogWarning("BitacoraEventosQueue llena. Se descartó evento. Tipo={Tipo} TraceId={TraceId}",
                        elEventoDeBitacoraLimpio.Tipo, elEventoDeBitacoraLimpio.TraceId);
                }
            }
            catch (Exception laExcepcion)
            {
                _elRegistrador.LogError(laExcepcion, "Fallo en EnqueueEvento (bitácora eventos). Se ignora para no afectar request.");
            }
        }

        private static BitacoraEvento LimpiarEventoDeBitacora(BitacoraEvento eventoDeBitacora)
        {
            // Nota: evita truncation en SQL (string/binary would be truncated)
            return new BitacoraEvento
            {
                Modulo = ObtenerTextoNoVacioYLimitado(eventoDeBitacora.Modulo, 30, "NA"),
                Servicio = LimitarTexto(eventoDeBitacora.Servicio, 60),
                Ambiente = LimitarTexto(eventoDeBitacora.Ambiente, 20),

                Tipo = ObtenerTextoNoVacioYLimitado(eventoDeBitacora.Tipo, 50, "EVENTO"),
                Nivel = AjustarNivel(eventoDeBitacora.Nivel),

                Mensaje = LimitarTexto(eventoDeBitacora.Mensaje, 500),

                TraceId = LimitarTexto(eventoDeBitacora.TraceId, 64),
                CorrelationId = LimitarTexto(eventoDeBitacora.CorrelationId, 64),

                IdUsuario = eventoDeBitacora.IdUsuario,

                EntidadTipo = LimitarTexto(eventoDeBitacora.EntidadTipo, 60),
                EntidadId = LimitarTexto(eventoDeBitacora.EntidadId, 80),

                ReferenceId = LimitarTexto(eventoDeBitacora.ReferenceId, 80),
                Proveedor = LimitarTexto(eventoDeBitacora.Proveedor, 30),

                PayloadJson = eventoDeBitacora.PayloadJson,

                IdUsuarioReg = eventoDeBitacora.IdUsuarioReg <= 0 ? 1 : eventoDeBitacora.IdUsuarioReg
            };
        }

        private static byte AjustarNivel(byte nivel)
        {
            // CK_BV_NIVEL: 1,2,3
            if (nivel < 1) return 1;
            if (nivel > 3) return 3;
            return nivel;
        }

        private static string? LimitarTexto(string? texto, int longitudMaxima)
        {
            if (string.IsNullOrWhiteSpace(texto)) return texto;
            texto = texto.Trim();
            return texto.Length <= longitudMaxima ? texto : texto.Substring(0, longitudMaxima);
        }

        private static string ObtenerTextoNoVacioYLimitado(string? texto, int longitudMaxima, string valorPorDefecto)
        {
            var elTextoLimitado = LimitarTexto(texto, longitudMaxima);
            return string.IsNullOrWhiteSpace(elTextoLimitado) ? valorPorDefecto : elTextoLimitado!;
        }
    }
}
