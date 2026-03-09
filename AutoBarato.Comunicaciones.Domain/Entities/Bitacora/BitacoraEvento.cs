namespace AutoBarato.Comunicaciones.Domain.Entities.Bitacora
{
    public sealed class BitacoraEvento
    {
        // Tabla: bitacora.t_ab_bitacora_evento

        public string Modulo { get; set; } = default!;           // bv_modulo varchar(30) NOT NULL
        public string? Servicio { get; set; }                    // bv_servicio varchar(60)
        public string? Ambiente { get; set; }                    // bv_ambiente varchar(20)

        public string Tipo { get; set; } = default!;             // bv_tipo varchar(50) NOT NULL
        public byte Nivel { get; set; } = 1;                     // bv_nivel tinyint NOT NULL (CK 1..3)

        public string? Mensaje { get; set; }                     // bv_mensaje nvarchar(500)

        public string? TraceId { get; set; }                     // bv_trace_id varchar(64)
        public string? CorrelationId { get; set; }               // bv_correlation_id varchar(64)

        public int? IdUsuario { get; set; }                      // bv_id_usuario int

        public string? EntidadTipo { get; set; }                 // bv_entidad_tipo varchar(60)
        public string? EntidadId { get; set; }                   // bv_entidad_id varchar(80)

        public string? ReferenceId { get; set; }                 // bv_reference_id varchar(80)
        public string? Proveedor { get; set; }                   // bv_proveedor varchar(30)

        public string? PayloadJson { get; set; }                 // bv_payload_json nvarchar(max)

        public int IdUsuarioReg { get; set; } = 1;               // bv_id_usuario_reg int NOT NULL (luego JWT)
    }
}
