using AutoBarato.Comunicaciones.Application.DTOs;
using AutoBarato.Comunicaciones.Application.DTOs.Common;
using AutoBarato.Comunicaciones.Application.Interfaces;
using AutoBarato.Comunicaciones.Domain.Entities;
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

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ApiFilesService> _logger;
        private readonly IConfiguration _configuration;


        public ApiFilesService(IHttpClientFactory httpClientFactory, ILogger<ApiFilesService> logger, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<List<UploadFilesResponse>> ProcesarArchivosAutoConFailover(UploadMediaRequest request)
        {
            var servers = _configuration.GetSection("ApiArchivos:Servers").Get<List<string>>();
            Exception? lastException = null;

            foreach (var serverUrl in servers)
            {
                try
                {
                    var client = _httpClientFactory.CreateClient();
                    client.BaseAddress = new Uri(serverUrl);

                    using var formData = new MultipartFormDataContent();

                    // Agregar el destino del folder
                    formData.Add(new StringContent(request.DestinoFolderBucket), "DestinoFolderBucket");

                    // Procesar cada archivo con su IdTipoArchivo correspondiente
                    if (request.Files != null)
                    {
                        for (int i = 0; i < request.Files.Count; i++)
                        {
                            var file = request.Files[i];
                            using var stream = new MemoryStream();
                            await file.File.CopyToAsync(stream);
                            var fileContent = new ByteArrayContent(stream.ToArray());

                            // Agregar el archivo y el IdTipoArchivo
                            formData.Add(fileContent, $"Files[{i}].File", file.File.FileName);
                            formData.Add(new StringContent(file.IdTipoArchivo.ToString()), $"Files[{i}].IdTipoArchivo");
                            //formData.Add(new StringContent(file.IdOrden.ToString()), $"Files[{i}].IdOrden");
                        }
                    }

                    var response = await client.PostAsync("api/Files/uploadMedia", formData);

                    if (response.IsSuccessStatusCode)
                    {
                        var apiResponse = await response.Content.ReadFromJsonAsync<Response<List<UploadFilesResponse>>>();


                        if (apiResponse?.Success == true && apiResponse.Data != null)
                        {
                            var uploadResponses = apiResponse.Data;
                            _logger.LogInformation("Archivos procesados exitosamente en servidor {ServerUrl}", serverUrl);
                            return uploadResponses;
                        }

                        _logger.LogWarning("Respuesta exitosa pero con errores del servidor {ServerUrl}: {Message}",
                            serverUrl, apiResponse?.Message ?? "Sin mensaje");
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        _logger.LogWarning("Error en la respuesta del servidor {ServerUrl}. StatusCode: {StatusCode}, Content: {Content}",
                            serverUrl, response.StatusCode, errorContent);
                    }
                }
                catch (Exception ex)
                {
                    lastException = ex;
                    _logger.LogWarning(ex, "Error al procesar archivos en servidor {ServerUrl}. Intentando siguiente servidor.", serverUrl);
                    continue;
                }
            }

            _logger.LogError(lastException, "Todos los servidores de archivos fallaron al procesar los archivos");
            throw new ServiceException("No se pudieron procesar los archivos en ningún servidor disponible", lastException);
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
