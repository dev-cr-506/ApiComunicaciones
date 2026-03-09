namespace AutoBarato.Comunicaciones.Infrastructure.Models.Common
{
    public class RespuestaApiExterna<T>
    {
        public int Status { get; set; }
        public bool Success { get; set; }
        public string? Message { get; set; }
        public List<ErrorApiExterna>? Errors { get; set; }
        public T? Data { get; set; }
    }

    public class ErrorApiExterna
    {
        public int ErrorCode { get; set; }
        public string? Message { get; set; }
    }
}
