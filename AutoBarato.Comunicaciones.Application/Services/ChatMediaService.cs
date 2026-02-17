using AutoBarato.Comunicaciones.Application.DTOs.Common;
using AutoBarato.Comunicaciones.Application.Interfaces;
using AutoBarato.Comunicaciones.Domain.Configuration;
using AutoBarato.Comunicaciones.Domain.Entities;
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
        private readonly JwtSettings _jwtSettings;
        private readonly StorageSettings _storageSettings;
        private readonly IApiFilesService _apiFilesService;


        public ChatMediaService(IOptions<JwtSettings> jwtSettings, IOptions<StorageSettings> storageSettings, IApiFilesService apiFilesService)
        {
            _jwtSettings = jwtSettings.Value;
            _storageSettings = storageSettings.Value;
            _apiFilesService = apiFilesService;
        }

        public async Task<ChatMediaResult> UploadAsync(
        IFormFile file,
        int userId,
        Guid? conversationId = null)
        {
            // ⚠️ Implementación de ejemplo mínima.
            // Luego aquí metes tu lógica real (S3, Azure Blob, etc.)

            // Guardar el archivo localmente, por ejemplo en wwwroot/chat-media
            var uploadsRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "chat-media");
            Directory.CreateDirectory(uploadsRoot);

            var extension = Path.GetExtension(file.FileName);
            var fileName = $"{Guid.NewGuid()}{extension}";
            var fullPath = Path.Combine(uploadsRoot, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            List<(int IdTipoArchivo, string FileUrl)> archivosSubidos = new();
            var bucketConfig = _storageSettings.Buckets[_storageSettings.DefaultBucket];

            var uploadRequest = new UploadMediaRequest
            {
                Files = (List<UploadFileRequest>)file,
                DestinoFolderBucket = $"{bucketConfig.BasePath}/{bucketConfig.Folder}/{conversationId}"

            };

            var filesResponses = await _apiFilesService.ProcesarArchivosAutoConFailover(uploadRequest);

            if (filesResponses?.Any() == true)
            {
                archivosSubidos = filesResponses
                    .Select(f => (IdTipoArchivo: f.IdTipoArchivo, FileUrl: f.FileUrl))
                    .ToList();
            }


            // Construir una URL "pública" básica (ajusta según tu hosting/reverse proxy)
            var mediaUrl = archivosSubidos.FirstOrDefault().FileUrl;

            // Detección simplificada de tipo
            var mediaType = file.ContentType.StartsWith("video", StringComparison.OrdinalIgnoreCase)
                ? "VIDEO"
                : "IMAGE";

            // De momento no generamos thumbnail
            string? thumbnailUrl = null;

            return new ChatMediaResult(
                mediaUrl,
                thumbnailUrl,
                mediaType
            );
        }
    }
}
