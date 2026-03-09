using AutoBarato.Comunicaciones.Application.DTOs.Request.Archivos;
using AutoBarato.Comunicaciones.Application.Interfaces;
using AutoBarato.Comunicaciones.Domain.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace AutoBarato.Comunicaciones.Application.Services.Chat
{
    public class ChatMediaService : IChatMediaService
    {
        private readonly ConfiguracionDeBucketsDeAlmacenamiento _laConfiguracionDeAlmacenamiento;
        private readonly IApiGatewayService _elServicioDeApiGateway;

        public ChatMediaService(
            IOptions<ConfiguracionDeBucketsDeAlmacenamiento> configuracionDeAlmacenamiento,
            IApiGatewayService servicioDeArchivosApi)
        {
            _laConfiguracionDeAlmacenamiento = configuracionDeAlmacenamiento.Value;
            _elServicioDeApiGateway = servicioDeArchivosApi;
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

            List<(int IdTipoArchivo, string FileUrl)> lasRespuestasDeArchivosSubidos = new();
            var laConfiguracionDelBucket = _laConfiguracionDeAlmacenamiento.Buckets[_laConfiguracionDeAlmacenamiento.DefaultBucket];

            var losArchivosParaCargar = new List<UploadFileRequest>();

            var laSolicitudDeArchivo = new UploadFileRequest
            {
                AbrirContenido = () => archivo.OpenReadStream(),
                NombreArchivo = archivo.FileName,
                TipoContenido = archivo.ContentType,
                IdTipoArchivo = 1
            };

            losArchivosParaCargar.Add(laSolicitudDeArchivo);

            var laSolicitudDeCargaDeMedios = new UploadMediaRequest
            {
                Files = losArchivosParaCargar,
                DestinoFolderBucket = $"{laConfiguracionDelBucket.BasePath}/{laConfiguracionDelBucket.Folder}/{idConversacion}"
            };


            var laRespuestaDeArchivos = await _elServicioDeApiGateway.ProcesarArchivosAutoConFailoverAsync(laSolicitudDeCargaDeMedios);

            if (laRespuestaDeArchivos?.Any() == true)
            {
                lasRespuestasDeArchivosSubidos = laRespuestaDeArchivos
                    .Select(archivoSubido => (IdTipoArchivo: archivoSubido.IdTipoArchivo, FileUrl: archivoSubido.FileUrl))
                    .ToList();
            }

            var laUrlDelMedio = lasRespuestasDeArchivosSubidos.FirstOrDefault().FileUrl;

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