namespace AutoBarato.Comunicaciones.Domain.Entities.Bitacora
{
    public sealed class BitacoraEvento
    {
        

        public string Modulo { get; set; } = default!;          
        public string? Servicio { get; set; }                   
        public string? Ambiente { get; set; }                   

        public string Tipo { get; set; } = default!;           
        public byte Nivel { get; set; } = 1;                    

        public string? Mensaje { get; set; }                     

        public string? TraceId { get; set; }                    
        public string? CorrelationId { get; set; }              

        public int? IdUsuario { get; set; }                      

        public string? EntidadTipo { get; set; }                
        public string? EntidadId { get; set; }                  

        public string? ReferenceId { get; set; }               
        public string? Proveedor { get; set; }                   

        public string? PayloadJson { get; set; }                 

        public int IdUsuarioReg { get; set; } = 1;               
    }
}
