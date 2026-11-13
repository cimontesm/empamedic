using backWeb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backWeb.Data.Configurations
{
    public class DetallePlantillaRecetaConfiguration : IEntityTypeConfiguration<DetallePlantillaReceta>
    {
        public void Configure(EntityTypeBuilder<DetallePlantillaReceta> builder)
        {
            builder.ToTable("DetallesPlantillaReceta");

            builder.HasKey(d => d.IdDetallePlantilla);

            builder.Property(d => d.DosisSugerida).IsRequired().HasMaxLength(100);
            builder.Property(d => d.FrecuenciaSugerida).IsRequired().HasMaxLength(100);
            builder.Property(d => d.DuracionSugerida).IsRequired().HasMaxLength(100);
            builder.Property(d => d.IndicacionesSugeridas).HasMaxLength(500);

            // Restrict: no se puede borrar un medicamento que está en alguna plantilla.
            builder.HasOne(d => d.Medicamento)
                .WithMany(m => m.DetallesPlantilla)
                .HasForeignKey(d => d.MedicamentoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
