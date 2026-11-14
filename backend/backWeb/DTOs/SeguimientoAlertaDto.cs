using backWeb.Models;

namespace backWeb.DTOs
{
    /// <summary>Alerta de seguimiento tal como la recibe la web y la app móvil.</summary>
    public class SeguimientoAlertaDto
    {
        public int IdAlerta { get; set; }
        public int IdColaborador { get; set; }
        public int IdMedicoResponsable { get; set; }
        public int? IdConsultaMedica { get; set; }
        public DateTime FechaControl { get; set; }
        public string Motivo { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public bool Vencida { get; set; }
        public DateTime? FechaResolucion { get; set; }
        public string? ObservacionResolucion { get; set; }

        public static SeguimientoAlertaDto Desde(SeguimientoAlerta alerta, DateTime hoy) => new()
        {
            IdAlerta = alerta.IdAlerta,
            IdColaborador = alerta.IdColaborador,
            IdMedicoResponsable = alerta.IdMedicoResponsable,
            IdConsultaMedica = alerta.IdConsultaMedica,
            FechaControl = alerta.FechaControl,
            Motivo = alerta.Motivo,
            Estado = alerta.Estado.ToString(),
            Vencida = alerta.Estado == EstadoAlerta.Pendiente && alerta.FechaControl.Date < hoy.Date,
            FechaResolucion = alerta.FechaResolucion,
            ObservacionResolucion = alerta.ObservacionResolucion
        };
    }
}
