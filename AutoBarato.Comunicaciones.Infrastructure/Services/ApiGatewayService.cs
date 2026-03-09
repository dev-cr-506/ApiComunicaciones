using AutoBarato.Comunicaciones.Application.DTOs.Request.Archivos;
using AutoBarato.Comunicaciones.Application.DTOs.Response.Archivos;
using AutoBarato.Comunicaciones.Application.Interfaces;
using AutoBarato.Comunicaciones.Domain.Exceptions;
using AutoBarato.Comunicaciones.Infrastructure.Models.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace AutoBarato.Comunicaciones.Infrastructure.Services
{
    public class ApiGatewayService : IApiGatewayService
    {
        private readonly IHttpClientFactory _laFabricaDeClientesHttp;
        private readonly ILogger<ApiGatewayService> _elRegistrador;
        private readonly IConfiguration _laConfiguracion;

        public ApiGatewayService(IHttpClientFactory fabricaDeClientesHttp,ILogger<ApiGatewayService> registrador,IConfiguration configuracion)
        {
            _laFabricaDeClientesHttp = fabricaDeClientesHttp;
            _elRegistrador = registrador;
            _laConfiguracion = configuracion;
        }

        public async Task<List<UploadFilesResponse>> ProcesarArchivosAutoConFailoverAsync(UploadMediaRequest solicitud)
        {
            var laListaDeServidores = _laConfiguracion.GetSection("ApiArchivos:Servers").Get<List<string>>() ?? new();
            Exception? laUltimaExcepcion = null;

            foreach (var laUrlDelServidor in laListaDeServidores)
            {
                try
                {
                    var elClienteHttp = _laFabricaDeClientesHttp.CreateClient();
                    elClienteHttp.BaseAddress = new Uri(laUrlDelServidor);

                    using var elFormularioMultipart = new MultipartFormDataContent();
                    var losFlujosAbiertos = new List<Stream>();

                    try
                    {
                        elFormularioMultipart.Add(
                            new StringContent(solicitud.DestinoFolderBucket),
                            "DestinoFolderBucket"
                        );

                        if (solicitud.Files != null)
                        {
                            for (int i = 0; i < solicitud.Files.Count; i++)
                            {
                                var elArchivo = solicitud.Files[i];

                                if (elArchivo.AbrirContenido != null)
                                {
                                    var elFlujoDelArchivo = elArchivo.AbrirContenido();
                                    losFlujosAbiertos.Add(elFlujoDelArchivo);

                                    var elContenidoDelArchivo = new StreamContent(elFlujoDelArchivo);

                                    if (!string.IsNullOrWhiteSpace(elArchivo.TipoContenido))
                                    {
                                        elContenidoDelArchivo.Headers.ContentType =
                                            new MediaTypeHeaderValue(elArchivo.TipoContenido);
                                    }

                                    elFormularioMultipart.Add(
                                        elContenidoDelArchivo,
                                        $"Files[{i}].File",
                                        elArchivo.NombreArchivo ?? $"archivo_{i}"
                                    );
                                }

                                elFormularioMultipart.Add(
                                    new StringContent(elArchivo.IdTipoArchivo.ToString()),
                                    $"Files[{i}].IdTipoArchivo"
                                );

                                if (elArchivo.IdOrden.HasValue)
                                {
                                    elFormularioMultipart.Add(
                                        new StringContent(elArchivo.IdOrden.Value.ToString()),
                                        $"Files[{i}].IdOrden"
                                    );
                                }

                                if (!string.IsNullOrWhiteSpace(elArchivo.ObjectKey))
                                {
                                    elFormularioMultipart.Add(
                                        new StringContent(elArchivo.ObjectKey),
                                        $"Files[{i}].ObjectKey"
                                    );
                                }
                            }
                        }

                        var laRespuestaHttp = await elClienteHttp.PostAsync("api/Files/UploadMedia", elFormularioMultipart);

                        if (laRespuestaHttp.IsSuccessStatusCode)
                        {
                            var laRespuestaApiExterna = await laRespuestaHttp.Content.ReadFromJsonAsync<RespuestaApiExterna<List<UploadFilesResponse>>>();

                            if (laRespuestaApiExterna?.Success == true && laRespuestaApiExterna.Data != null)
                            {
                                _elRegistrador.LogInformation(
                                    "Archivos procesados exitosamente en servidor {ServerUrl}",
                                    laUrlDelServidor);

                                return laRespuestaApiExterna.Data;
                            }

                            _elRegistrador.LogWarning(
                                "Respuesta exitosa pero con errores del servidor {ServerUrl}: {Message}",
                                laUrlDelServidor,
                                laRespuestaApiExterna?.Message ?? "Sin mensaje");
                        }
                        else
                        {
                            var elContenidoDeError = await laRespuestaHttp.Content.ReadAsStringAsync();

                            _elRegistrador.LogWarning(
                                "Error en la respuesta del servidor {ServerUrl}. StatusCode: {StatusCode}, Content: {Content}",
                                laUrlDelServidor,
                                laRespuestaHttp.StatusCode,
                                elContenidoDeError);
                        }
                    }
                    finally
                    {
                        foreach (var elFlujo in losFlujosAbiertos)
                        {
                            elFlujo.Dispose();
                        }
                    }
                }
                catch (Exception excepcion)
                {
                    laUltimaExcepcion = excepcion;

                    _elRegistrador.LogWarning(
                        excepcion,
                        "Error al procesar archivos en servidor {ServerUrl}. Intentando siguiente servidor.",
                        laUrlDelServidor);
                }
            }

            _elRegistrador.LogError(
                laUltimaExcepcion,
                "Todos los servidores de archivos fallaron al procesar los archivos");

            throw new ExcepcionDeServicio(
                "No se pudieron procesar los archivos en ningún servidor disponible",
                laUltimaExcepcion);
        }

        public async Task<List<DeleteFilesResponse>> EliminarArchivosS3Async(DeleteFilesRequest solicitud)
        {
            var laListaDeServidores = _laConfiguracion.GetSection("ApiArchivos:Servers").Get<List<string>>() ?? new();
            Exception? laUltimaExcepcion = null;

            foreach (var laUrlDelServidor in laListaDeServidores)
            {
                try
                {
                    var elClienteHttp = _laFabricaDeClientesHttp.CreateClient();
                    elClienteHttp.BaseAddress = new Uri(laUrlDelServidor);

                    var laSolicitudHttp = new HttpRequestMessage(HttpMethod.Delete, "api/Files/DeleteFiles")
                    {
                        Content = JsonContent.Create(solicitud)
                    };

                    var laRespuestaHttp = await elClienteHttp.SendAsync(laSolicitudHttp);

                    if (laRespuestaHttp.IsSuccessStatusCode)
                    {
                        var laRespuestaApiExterna = await laRespuestaHttp.Content.ReadFromJsonAsync<RespuestaApiExterna<List<DeleteFilesResponse>>>();

                        if (laRespuestaApiExterna?.Success == true && laRespuestaApiExterna.Data != null)
                        {
                            _elRegistrador.LogInformation(
                                "Archivos eliminados exitosamente en servidor {ServerUrl}",
                                laUrlDelServidor);

                            return laRespuestaApiExterna.Data;
                        }

                        _elRegistrador.LogWarning(
                            "Respuesta exitosa pero con errores del servidor {ServerUrl}: {Message}",
                            laUrlDelServidor,
                            laRespuestaApiExterna?.Message ?? "Sin mensaje");
                    }
                    else
                    {
                        var elContenidoDeError = await laRespuestaHttp.Content.ReadAsStringAsync();

                        _elRegistrador.LogWarning(
                            "Error en la respuesta del servidor {ServerUrl}. StatusCode: {StatusCode}, Content: {Content}",
                            laUrlDelServidor,
                            laRespuestaHttp.StatusCode,
                            elContenidoDeError);
                    }
                }
                catch (Exception excepcion)
                {
                    laUltimaExcepcion = excepcion;

                    _elRegistrador.LogWarning(
                        excepcion,
                        "Error al eliminar archivos en servidor {ServerUrl}. Intentando siguiente servidor.",
                        laUrlDelServidor);
                }
            }

            _elRegistrador.LogError(
                laUltimaExcepcion,
                "Todos los servidores de archivos fallaron al eliminar los archivos");

            throw new ExcepcionDeServicio(
                "No se pudieron eliminar los archivos en ningún servidor disponible",
                laUltimaExcepcion);
        }
    }
}