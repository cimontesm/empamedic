using backWeb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backWeb.Data.Configurations
{
    public class MedicamentoConfiguration : IEntityTypeConfiguration<Medicamento>
    {
        public void Configure(EntityTypeBuilder<Medicamento> builder)
        {
            builder.ToTable("Medicamentos");

            builder.HasKey(m => m.IdMedicamento);

            builder.Property(m => m.NombreComercial).IsRequired().HasMaxLength(150);
            builder.Property(m => m.PrincipioActivo).IsRequired().HasMaxLength(150);
            builder.Property(m => m.Presentacion).IsRequired().HasMaxLength(100);
            builder.Property(m => m.Concentracion).IsRequired().HasMaxLength(50);

            // Para el buscador de fármacos en la pantalla de consulta.
            builder.HasIndex(m => m.NombreComercial);
        }
    }
}
