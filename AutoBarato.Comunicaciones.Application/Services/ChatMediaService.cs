using AutoBarato.Comunicaciones.Application.DTOs.Common;
using AutoBarato.Comunicaciones.Application.Interfaces;
using AutoBarato.Comunicaciones.Domain.Configuration;
using AutoBarato.Comunicaciones.Domain.Entities;
using AutoBarato.Comunicaciones.Domain.Entities.Archivos;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoBarato.Comunicaciones.Application.Services
{
    public class ChatMediaService : IChatMediaService
    {
        private readonly ConfiguracionDeBucketsDeAlmacenamiento _laConfiguracionDeAlmacenamiento;
        private readonly IApiFilesService _elServicioDeArchivosApi;

        public ChatMediaService( IOptions<ConfiguracionDeBucketsDeAlmacenamiento> configuracionDeAlmacenamiento, IApiFilesService servicioDeArchivosApi)
        {
            _laConfiguracionDeAlmacenamiento = configuracionDeAlmacenamiento.Value;
            _elServicioDeArchivosApi = servicioDeArchivosApi;
        }

        public async Task<ChatMediaResult> UploadAsync(
            IFormFile archivo,
            int idUsuario,
            Guid? idConversacion = null)
        {
            var laRutaRaizDeCargas = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "chat-media");
            Directory.CreateDirectory(laRutaRaizDeCargas);

            var laExtension = Path.GetExtension(archivo.FileName);
            var elNombreDelArchivo = $"{Guid.NewGuid()}{laExtension}";
            var laRutaCompleta = Path.Combine(laRutaRaizDeCargas, elNombreDelArchivo);

            using (var elFlujoDeArchivo = new FileStream(laRutaCompleta, FileMode.Create))
            {
                await archivo.CopyToAsync(elFlujoDeArchivo);
            }

            List<(int IdTipoArchivo, string FileUrl)> losArchivosSubidos = new();
            var laConfiguracionDelBucket = _laConfiguracionDeAlmacenamiento.Buckets[_laConfiguracionDeAlmacenamiento.DefaultBucket];
            List<UploadFileRequest>? losArchivos = new List<UploadFileRequest>();

            var elArchivoDeCarga = new UploadFileRequest
            {
                File = archivo,
                IdTipoArchivo = 1
            };

            losArchivos!.Add(elArchivoDeCarga);

            var laSolicitudDeCarga = new UploadMediaRequest
            {
                Files = losArchivos,
                DestinoFolderBucket = $"{laConfiguracionDelBucket.BasePath}/{laConfiguracionDelBucket.Folder}/{idConversacion}"
            };

            var lasRespuestasDeArchivos = await _elServicioDeArchivosApi.ProcesarArchivosAutoConFailover(laSolicitudDeCarga);

            if (lasRespuestasDeArchivos?.Any() == true)
            {
                losArchivosSubidos = lasRespuestasDeArchivos
                    .Select(archivoSubido => (IdTipoArchivo: archivoSubido.IdTipoArchivo, FileUrl: archivoSubido.FileUrl))
                    .ToList();
            }

            var laUrlDelMedio = losArchivosSubidos.FirstOrDefault().FileUrl;

            var elTipoDeMedio = archivo.ContentType.StartsWith("video", StringComparison.OrdinalIgnoreCase)
                ? "VIDEO"
                : "IMAGE";

            string? laUrlDeMiniatura = null;

            return new ChatMediaResult(
                laUrlDelMedio,
                laUrlDeMiniatura,
                elTipoDeMedio
            );
        }
    }
}