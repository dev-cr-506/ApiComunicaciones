namespace AutoBarato.Comunicaciones.Infrastructure.Bitacora.Evento
{
    public sealed class BitacoraEventosOptions
    {
        public bool Enabled { get; set; } = true;
        public int FlushIntervalMs { get; set; } = 1000;
        public int MaxBatchSize { get; set; } = 50;
        public int QueueCapacity { get; set; } = 2000;
    }
}
