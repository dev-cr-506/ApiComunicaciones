using System.Net;

namespace AutoBarato.Comunicaciones.Api.Models.Common
{
    /// <summary>
    /// Clase genérica para manejar respuestas en la API.
    /// </summary>
    public class Response<T>
    {
        public HttpStatusCode Status { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<ErrorResponse>? Errors { get; set; }
        public T Data { get; set; }

        /// <summary>
        /// Metadatos opcionales (paginación, seed, etc.). No rompe compatibilidad si no se usa.
        /// </summary>
        public ResponseMeta? Meta { get; set; }

        public Response()
        {
            Errors = new List<ErrorResponse>();
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
        public static Response<T> ErrorResponse(HttpStatusCode status, string message, List<ErrorResponse>? errors = null)
        {
            return new Response<T>
            {
                Status = status,
                Success = false,
                Message = message,
                Errors = errors ?? new List<ErrorResponse>(),
                Data = default
            };
        }
    }
}
