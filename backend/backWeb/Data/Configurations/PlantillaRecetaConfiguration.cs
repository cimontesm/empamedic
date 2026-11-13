using backWeb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backWeb.Data.Configurations
{
    public class PlantillaRecetaConfiguration : IEntityTypeConfiguration<PlantillaReceta>
    {
        public void Configure(EntityTypeBuilder<PlantillaReceta> builder)
        {
            builder.ToTable("PlantillasReceta");

            builder.HasKey(p => p.IdPlantilla);

            builder.Property(p => p.NombrePlantilla).IsRequired().HasMaxLength(100);
            builder.Property(p => p.DiagnosticoAsociado).IsRequired().HasMaxLength(200);
            builder.Property(p => p.Descripcion).HasMaxLength(500);

            builder.HasIndex(p => p.NombrePlantilla).IsUnique();

            // Composición: los ítems pertenecen a la plantilla y se borran con ella.
            builder.HasMany(p => p.Items)
                .WithOne(d => d.Plantilla)
                .HasForeignKey(d => d.PlantillaRecetaId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
