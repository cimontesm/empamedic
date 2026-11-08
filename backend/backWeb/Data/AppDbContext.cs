using Microsoft.EntityFrameworkCore;
using backWeb.Models;

namespace backWeb.Data
{
    /// <summary>
    /// Único DbContext del sistema EMPAMEDIC (SQL Server 2019).
    /// Cada integrante agrega aquí los DbSet de sus clases y su configuración,
    /// y luego crea la migración con: dotnet ef migrations add NombreDelCambio
    /// </summary>
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Usuario> Usuarios => Set<Usuario>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Aplica automáticamente las clases IEntityTypeConfiguration<T> de este proyecto.
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
