using AutoBarato.Comunicaciones.Domain.Entities.Comunicaciones;
using Microsoft.EntityFrameworkCore;


namespace AutoBarato.Comunicaciones.Infrastructure.DataAccess
{
    public class ComunicacionesDbContext : DbContext
    {
        public ComunicacionesDbContext(DbContextOptions<ComunicacionesDbContext> opciones) : base(opciones) { }

        protected override void OnModelCreating(ModelBuilder constructorDeModelo)
        {
            base.OnModelCreating(constructorDeModelo);
            constructorDeModelo.Entity<ChatConversacion>().HasNoKey(); 
            constructorDeModelo.Entity<ChatMensaje>().HasNoKey(); 

            constructorDeModelo.ApplyConfigurationsFromAssembly(typeof(ComunicacionesDbContext).Assembly);
        }
    }
}
