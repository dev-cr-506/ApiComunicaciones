using AutoBarato.Comunicaciones.Application.DTOs.Common;
using AutoBarato.Comunicaciones.Application.DTOs.Response.Archivos;
using AutoBarato.Comunicaciones.Application.Interfaces;
using AutoBarato.Comunicaciones.Domain.Entities.Archivos;
using AutoBarato.Comunicaciones.Domain.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace AutoBarato.Comunicaciones.Application.Services
{
    public class ApiFilesService : IApiFilesService
    {
        private readonly IHttpClientFactory _laFabricaDeClientesHttp;
        private readonly ILogger<ApiFilesService> _elRegistrador;
        private readonly IConfiguration _laConfiguracion;

        public ApiFilesService(IHttpClientFactory fabricaDeClientesHttp, ILogger<ApiFilesService> registrador, IConfiguration configuracion)
        {
            _laFabricaDeClientesHttp = fabricaDeClientesHttp;
            _elRegistrador = registrador;
            _laConfiguracion = configuracion;
        }

        public async Task<List<UploadFilesResponse>> ProcesarArchivosAutoConFailover(UploadMediaRequest solicitudDeCargaDeMedios)
        {
            var losServidores = _laConfiguracion.GetSection("ApiArchivos:Servers").Get<List<string>>();
            Exception? laUltimaExcepcion = null;

            foreach (var laUrlDelServidor in losServidores)
            {
                try
                {
                    var elClienteHttp = _laFabricaDeClientesHttp.CreateClient();
                    elClienteHttp.BaseAddress = new Uri(laUrlDelServidor);

                    using var losDatosDelFormulario = new MultipartFormDataContent();

                    losDatosDelFormulario.Add(new StringContent(solicitudDeCargaDeMedios.DestinoFolderBucket), "DestinoFolderBucket");

                    if (solicitudDeCargaDeMedios.Files != null)
                    {
                        for (int indice = 0; indice < solicitudDeCargaDeMedios.Files.Count; indice++)
                        {
                            var elArchivo = solicitudDeCargaDeMedios.Files[indice];
                            using var elFlujoDeArchivo = new MemoryStream();
                            await elArchivo.File.CopyToAsync(elFlujoDeArchivo);
                            var elContenidoDelArchivo = new ByteArrayContent(elFlujoDeArchivo.ToArray());

                            losDatosDelFormulario.Add(elContenidoDelArchivo, $"Files[{indice}].File", elArchivo.File.FileName);
                            losDatosDelFormulario.Add(new StringContent(elArchivo.IdTipoArchivo.ToString()), $"Files[{indice}].IdTipoArchivo");
                            //formData.Add(new StringContent(file.IdOrden.ToString()), $"Files[{i}].IdOrden");
                        }
                    }

                    var laRespuestaHttp = await elClienteHttp.PostAsync("api/Files/uploadMedia", losDatosDelFormulario);

                    if (laRespuestaHttp.IsSuccessStatusCode)
                    {
                        var laRespuestaDeLaApi = await laRespuestaHttp.Content.ReadFromJsonAsync<Response<List<UploadFilesResponse>>>();

                        if (laRespuestaDeLaApi?.Success == true && laRespuestaDeLaApi.Data != null)
                        {
                            var lasRespuestasDeCarga = laRespuestaDeLaApi.Data;
                            _elRegistrador.LogInformation("Archivos procesados exitosamente en servidor {ServerUrl}", laUrlDelServidor);
                            return lasRespuestasDeCarga;
                        }

                        _elRegistrador.LogWarning("Respuesta exitosa pero con errores del servidor {ServerUrl}: {Message}",
                            laUrlDelServidor, laRespuestaDeLaApi?.Message ?? "Sin mensaje");
                    }
                    else
                    {
                        var elContenidoDeError = await laRespuestaHttp.Content.ReadAsStringAsync();
                        _elRegistrador.LogWarning("Error en la respuesta del servidor {ServerUrl}. StatusCode: {StatusCode}, Content: {Content}",
                            laUrlDelServidor, laRespuestaHttp.StatusCode, elContenidoDeError);
                    }
                }
                catch (Exception laExcepcion)
                {
                    laUltimaExcepcion = laExcepcion;
                    _elRegistrador.LogWarning(laExcepcion, "Error al procesar archivos en servidor {ServerUrl}. Intentando siguiente servidor.", laUrlDelServidor);
                    continue;
                }
            }

            _elRegistrador.LogError(laUltimaExcepcion, "Todos los servidores de archivos fallaron al procesar los archivos");
            throw new ExcepcionDeServicio("No se pudieron procesar los archivos en ningún servidor disponible", laUltimaExcepcion);
        }

        public class FileUploadResult
        {
            public string FileName { get; set; } = string.Empty;
            public bool Success { get; set; }
            public string Message { get; set; } = string.Empty;
            public string ObjectKey { get; set; } = string.Empty;
        }
    }
}