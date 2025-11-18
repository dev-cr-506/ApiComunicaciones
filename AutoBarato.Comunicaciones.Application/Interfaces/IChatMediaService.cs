using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace AutoBarato.Comunicaciones.Application.Interfaces
{
    public interface IChatMediaService
    {
        Task<ChatMediaResult> UploadAsync(
          IFormFile file,
          int userId,
          Guid? conversationId = null);
    }
}
