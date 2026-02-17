using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AutoBarato.Comunicaciones.Application.DTOs.Common
{
    /// <summary>
    /// Clase genérica para manejar respuestas en la API.
    /// </summary>
    public class Response<T>
    {
        public HttpStatusCode Status { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<Error>? Errors { get; set; }
        public T Data { get; set; }
        public Response()
        {
            Errors = new List<Error>();
        }

        /// <summary>
        /// Método estático para crear una respuesta de éxito.
        /// </summary>
        public static Response<T> SuccessResponse(T data, string message = "Ejecución Correcta.")
        {
            return new Response<T>
            {
                Status = HttpStatusCode.OK,
                Success = true,
                Message = message,
                Data = data
            };
        }

        /// <summary>
        /// Método estático para crear una respuesta de error.
        /// </summary>
        public static Response<T> ErrorResponse(HttpStatusCode status, string message, List<Error>? errors = null)
        {
            return new Response<T>
            {
                Status = status,
                Success = false,
                Message = message,
                Errors = errors ?? new List<Error>(),
                Data = default
            };
        }
    }

    /// <summary>
    /// Modelo para manejar errores detallados.
    /// </summary>
    public class Error
    {
        public int ErrorCode { get; set; }
        public string Message { get; set; }
    }
}
