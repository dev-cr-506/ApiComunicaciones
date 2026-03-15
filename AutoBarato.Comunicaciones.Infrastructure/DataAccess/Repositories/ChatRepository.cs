using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoBarato.Comunicaciones.Domain.Entities.Comunicaciones;
using AutoBarato.Comunicaciones.Domain.Interfaces.Chat;
using AutoBarato.Comunicaciones.Infrastructure.DataAccess.Helpers;
using Microsoft.Data.SqlClient;

namespace AutoBarato.Comunicaciones.Infrastructure.DataAccess.Repositories
{
    public class ChatRepository : IChatRepository
    {
        private readonly EjecutorDeProcedimientosAlmacenados _elEjecutorDeProcedimientosAlmacenados;

        public ChatRepository(EjecutorDeProcedimientosAlmacenados ejecutorDeProcedimientosAlmacenados)
        {
            _elEjecutorDeProcedimientosAlmacenados = ejecutorDeProcedimientosAlmacenados;
        }

        public async Task<ChatConversacion?> CrearOObtenerConversacionesAsync(
            int idAuto,
            int idVendedor,
            int idComprador)
        {
            var losParametros = new[]
            {
                HelperDeParametrosSql.CrearEntero("@id_auto", idAuto),
                HelperDeParametrosSql.CrearEntero("@c_vendedor", idVendedor),
                HelperDeParametrosSql.CrearEntero("@c_comprador", idComprador)
            };

            var laConversacion = await _elEjecutorDeProcedimientosAlmacenados
                .EjecutarProcedimientoAlmacenadoUnicoAsync<ChatConversacion>(
                    "comunicaciones.Crear_Obtener_Conversacion",
                    losParametros);

            return laConversacion;
        }

        public async Task<IReadOnlyList<ChatConversacion>> ObtenerConversacionesUsuarioAsync(
            int idUsuario,
            int skip,
            int take)
        {
            var losParametros = new[]
            {
                HelperDeParametrosSql.CrearEntero("@id_usuario", idUsuario),
                HelperDeParametrosSql.CrearEntero("@skip", skip),
                HelperDeParametrosSql.CrearEntero("@take", take)
            };

            var lasConversaciones = await _elEjecutorDeProcedimientosAlmacenados
                .EjecutarProcedimientoAlmacenadoAsync<ChatConversacion>(
                    "comunicaciones.Consultar_Conversaciones_Usuario",
                    losParametros);

            return lasConversaciones;
        }

        public async Task<ChatConversacion?> ObtenerConversacionPorIdAsync(Guid idConversacion)
        {
            var losParametros = new[]
            {
                HelperDeParametrosSql.CrearGuid("@id_conversacion", idConversacion)
            };

            var laConversacion = await _elEjecutorDeProcedimientosAlmacenados
                .EjecutarProcedimientoAlmacenadoUnicoAsync<ChatConversacion>(
                    "comunicaciones.Consultar_Conversacion_Id",
                    losParametros);

            return laConversacion;
        }

        public async Task<IReadOnlyList<ChatMensaje>> ObtenerMensajesDeConversacionAsync(
            Guid idConversacion,
            int skip,
            int take)
        {
            var losParametros = new[]
            {
                HelperDeParametrosSql.CrearGuid("@id_conversacion", idConversacion),
                HelperDeParametrosSql.CrearEntero("@skip", skip),
                HelperDeParametrosSql.CrearEntero("@take", take)
            };

            var losMensajes = await _elEjecutorDeProcedimientosAlmacenados
                .EjecutarProcedimientoAlmacenadoAsync<ChatMensaje>(
                    "comunicaciones.Listar_Mensajes_Conversacion_Paginado",
                    losParametros);

            return losMensajes;
        }

        public async Task<ChatMensaje> InsertarMensajeAsync(
            Guid idConversacion,
            int remitenteId,
            string? texto,
            string tipoMensaje,
            string? mediaUrl,
            string? mediaThumbnailUrl,
            DateTime? fechaDeExpiracionDelMedio)
        {
            var losParametros = new[]
            {
                HelperDeParametrosSql.CrearGuid("@id_conversacion", idConversacion),
                HelperDeParametrosSql.CrearEntero("@m_remitente", remitenteId),
                HelperDeParametrosSql.CrearNvarchar("@m_texto", texto),
                HelperDeParametrosSql.CrearNvarchar("@m_tipo_mensaje", tipoMensaje, 50),
                HelperDeParametrosSql.CrearNvarchar("@m_media_url", mediaUrl),
                HelperDeParametrosSql.CrearNvarchar("@m_media_thumbnail_url", mediaThumbnailUrl),
                HelperDeParametrosSql.CrearFechaHoraNula("@m_media_expira_en", fechaDeExpiracionDelMedio)
            };

            var elMensaje = await _elEjecutorDeProcedimientosAlmacenados
                .EjecutarProcedimientoAlmacenadoUnicoAsync<ChatMensaje>(
                    "comunicaciones.Insertar_Mensaje",
                    losParametros);

            return elMensaje;
        }

        public async Task<ChatMensaje?> EditarMensajeAsync(
            Guid idMensaje,
            int remitenteId,
            string textoNuevo)
        {
            var losParametros = new[]
            {
                HelperDeParametrosSql.CrearGuid("@id_mensaje", idMensaje),
                HelperDeParametrosSql.CrearEntero("@m_remitente", remitenteId),
                HelperDeParametrosSql.CrearNvarchar("@nuevo_texto", textoNuevo, 1000)
            };

            var elMensaje = await _elEjecutorDeProcedimientosAlmacenados
                .EjecutarProcedimientoAlmacenadoUnicoAsync<ChatMensaje>(
                    "comunicaciones.Editar_Mensaje",
                    losParametros);

            return elMensaje;
        }

        public async Task MarcarMensajeComoLeidoAsync(
            Guid idConversacion,
            int idUsuario)
        {
            var losParametros = new SqlParameter[]
            {
        HelperDeParametrosSql.CrearGuid("@id_conversacion", idConversacion),
        HelperDeParametrosSql.CrearEntero("@id_usuario", idUsuario)
            };

            await _elEjecutorDeProcedimientosAlmacenados
                .EjecutarProcedimientoAlmacenadoSinResultadoAsync(
                    "comunicaciones.Marcar_Mensajes_Leidos",
                    losParametros);
        }
    }
}