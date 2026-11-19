namespace appEmpamedic.Models
{
    public enum EstadoAlerta
    {
        Pendiente,
        Atendida,
        Descartada
    }

    /// <summary>
    /// Recordatorio de control médico de un colaborador (RF08).
    /// Solo lo recibe el médico responsable del paciente.
    /// </summary>
    public class SeguimientoAlerta
    {
        public int Id { get; set; }
        public int ColaboradorId { get; set; }
        public string NombreColaborador { get; set; } = string.Empty;
        public int MedicoResponsableId { get; set; }
        public int? ConsultaMedicaId { get; set; }
        public DateTime FechaControl { get; set; }
        public string? Motivo { get; set; }
        public EstadoAlerta Estado { get; set; } = EstadoAlerta.Pendiente;
        public DateTime? FechaResolucion { get; set; }

        public bool Vencida => Estado == EstadoAlerta.Pendiente && FechaControl.Date < DateTime.Today;
    }
}
