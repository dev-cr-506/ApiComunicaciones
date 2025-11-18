using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoBarato.Comunicaciones.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace AutoBarato.Comunicaciones.Application.Services
{
    public class ChatMediaService : IChatMediaService
    {
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

            // Construir una URL "pública" básica (ajusta según tu hosting/reverse proxy)
            var mediaUrl = $"/chat-media/{fileName}";

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
