using AutoBarato.Comunicaciones.Domain.Entities.Comunicaciones;
using Microsoft.EntityFrameworkCore;


namespace AutoBarato.Comunicaciones.Infrastructure.DataAccess
{
    public class ComunicacionesDbContext : DbContext
    {
        public ComunicacionesDbContext(DbContextOptions<ComunicacionesDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ChatConversacion>().HasNoKey(); // Solo si es intencional
            modelBuilder.Entity<ChatMensaje>().HasNoKey(); // Solo si es intencional

            // 🔹 Aplica todas las configuraciones de entidades desde Infrastructure.Configurations
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ComunicacionesDbContext).Assembly);
        }
    }
}
