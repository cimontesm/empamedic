using backWeb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backWeb.Data.Configurations
{
    public class SeguimientoAlertaConfiguration : IEntityTypeConfiguration<SeguimientoAlerta>
    {
        public void Configure(EntityTypeBuilder<SeguimientoAlerta> builder)
        {
            builder.ToTable("SeguimientoAlertas");

            builder.HasKey(a => a.IdAlerta);

            builder.Property(a => a.Motivo)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(a => a.ObservacionResolucion)
                .HasMaxLength(500);

            // Se guarda como texto ("Pendiente", "Atendida"...) para que sea legible en SQL Server.
            builder.Property(a => a.Estado)
                .HasConversion<string>()
                .HasMaxLength(20);

            // Un usuario con alertas no se puede borrar; se desactiva (Usuario.Activo).
            builder.HasOne(a => a.MedicoResponsable)
                .WithMany()
                .HasForeignKey(a => a.IdMedicoResponsable)
                .OnDelete(DeleteBehavior.Restrict);

            // Consulta más frecuente: alertas pendientes de un médico ordenadas por fecha.
            builder.HasIndex(a => new { a.IdMedicoResponsable, a.Estado, a.FechaControl });

            builder.HasIndex(a => a.IdColaborador);
        }
    }
}
