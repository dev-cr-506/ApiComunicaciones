namespace AutoBarato.Comunicaciones.Infrastructure.Bitacora.Error
{
    public sealed class BitacoraErrorOptions
    {
        public bool Enabled { get; set; } = true;
        public int FlushIntervalMs { get; set; } = 1000;
        public int MaxBatchSize { get; set; } = 50;
        public int QueueCapacity { get; set; } = 2000;
    }
}
