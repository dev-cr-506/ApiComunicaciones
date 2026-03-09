namespace AutoBarato.Comunicaciones.Domain.Entities.Bitacora
{
    public sealed class BitacoraDeError
    {
        public string Modulo { get; set; } = default!;
        public string? Servicio { get; set; }
        public string? Ambiente { get; set; }
        public byte Severidad { get; set; } = 3; // 1=Info,2=Warn,3=Error,4=Critical
        public string? TraceId { get; set; }
        public string? CorrelationId { get; set; }
        public string? RequestId { get; set; }
        public string? HttpMethod { get; set; }
        public string? RequestPath { get; set; }
        public string? QueryString { get; set; }
        public string? RemoteIp { get; set; }
        public string? UserAgent { get; set; }
        public int? IdUsuario { get; set; } // TODO: reemplazar por JWT luego
        public string? EntidadTipo { get; set; }
        public string? EntidadId { get; set; }
        public string? ReferenceId { get; set; }
        public string? Proveedor { get; set; }
        public string ExceptionType { get; set; } = default!;
        public string Message { get; set; } = default!;
        public string? StackTrace { get; set; }
        public string? InnerMessage { get; set; }

        public string? MetadataJson { get; set; }

        // Trazabilidad estándar (según tu estándar de tablas)
        public int IdUsuarioReg { get; set; } = 1; // TODO: JWT
    }
}
