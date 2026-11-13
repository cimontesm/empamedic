using backWeb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backWeb.Data.Configurations
{
    public class MedicamentoConfiguration : IEntityTypeConfiguration<Medicamento>
    {
        public void Configure(EntityTypeBuilder<Medicamento> builder)
        {
            // La regla "stock nunca negativo" también se protege en la base de datos.
            builder.ToTable("Medicamentos", t =>
                t.HasCheckConstraint("CK_Medicamentos_Stock_NoNegativo", "[Stock] >= 0"));

            builder.HasKey(m => m.IdMedicamento);

            builder.Property(m => m.NombreComercial).IsRequired().HasMaxLength(150);
            builder.Property(m => m.PrincipioActivo).IsRequired().HasMaxLength(150);
            builder.Property(m => m.Presentacion).IsRequired().HasMaxLength(100);
            builder.Property(m => m.Concentracion).IsRequired().HasMaxLength(50);

            builder.Property(m => m.Stock).IsRequired();

            // Control de concurrencia optimista: columna rowversion de SQL Server.
            builder.Property(m => m.RowVersion).IsRowVersion();

            // Para el buscador de fármacos en la pantalla de consulta.
            builder.HasIndex(m => m.NombreComercial);
        }
    }
}
