using backWeb.Models;

namespace backWeb.Services
{
    /// <summary>
    /// Alertas y recordatorios de control médico (RF08).
    /// </summary>
    public interface ISeguimientoService
    {
        /// <summary>
        /// Programa un control para un colaborador. El responsable debe ser un médico o
        /// enfermero activo y la fecha no puede estar en el pasado.
        /// </summary>
        Task<ResultadoCreacion> CrearAsync(
            int idColaborador,
            int idMedicoResponsable,
            DateTime fechaControl,
            string motivo,
            int? idConsultaMedica = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Alertas pendientes del médico: las vencidas y las que vencen dentro de los días de anticipación.
        /// Nunca incluye alertas de otros médicos.
        /// </summary>
        Task<IReadOnlyList<SeguimientoAlerta>> ObtenerPendientesAsync(
            int idMedico,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Marca una alerta como atendida o descartada. Solo el médico responsable puede hacerlo.
        /// </summary>
        Task<ResultadoResolucion> ResolverAsync(
            int idAlerta,
            int idMedico,
            EstadoAlerta nuevoEstado,
            string? observacion,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Envía por push los recordatorios que entraron en la ventana de anticipación y aún no se
        /// notificaron. Cada alerta se notifica una sola vez. Devuelve cuántas se enviaron.
        /// </summary>
        Task<int> EnviarRecordatoriosAsync(CancellationToken cancellationToken = default);
    }
}
