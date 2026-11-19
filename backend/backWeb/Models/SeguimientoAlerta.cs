namespace backWeb.Models
{
    /// <summary>
    /// Recordatorio de un control médico programado para un colaborador (RF08).
    /// Solo el médico responsable la recibe y solo él puede resolverla.
    /// </summary>
    public class SeguimientoAlerta
    {
        public int IdAlerta { get; set; }

        /// <summary>
        /// Paciente del control. Todavía sin relación en la base de datos:
        /// se enlaza con Colaborador cuando el integrante 2 agregue esa entidad.
        /// </summary>
        public int IdColaborador { get; set; }

        /// <summary>Médico o enfermero que recibe la alerta (Usuario.IdUsuario).</summary>
        public int IdMedicoResponsable { get; set; }

        public Usuario? MedicoResponsable { get; set; }

        /// <summary>
        /// Consulta que originó el control, si la hay. Sin relación en la base de datos
        /// hasta que el integrante 3 agregue ConsultaMedica.
        /// </summary>
        public int? IdConsultaMedica { get; set; }

        public DateTime FechaControl { get; set; }

        public string Motivo { get; set; } = string.Empty;

        public EstadoAlerta Estado { get; set; } = EstadoAlerta.Pendiente;

        public DateTime FechaCreacion { get; set; }

        public DateTime? FechaResolucion { get; set; }

        public string? ObservacionResolucion { get; set; }

        /// <summary>
        /// Momento en que se envió la notificación push al médico.
        /// Sirve para no enviar el mismo recordatorio dos veces.
        /// </summary>
        public DateTime? FechaNotificacionPush { get; set; }
    }
}
