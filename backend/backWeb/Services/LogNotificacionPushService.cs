using backWeb.Models;

namespace backWeb.Services
{
    /// <summary>
    /// Implementación provisional: registra el recordatorio en el log en lugar de enviarlo.
    /// Se reemplaza por la implementación con Firebase Cloud Messaging cuando exista el proyecto de Firebase.
    /// </summary>
    public class LogNotificacionPushService : INotificacionPushService
    {
        private readonly ILogger<LogNotificacionPushService> _logger;

        public LogNotificacionPushService(ILogger<LogNotificacionPushService> logger)
        {
            _logger = logger;
        }

        public Task<bool> EnviarRecordatorioAsync(SeguimientoAlerta alerta, CancellationToken cancellationToken = default)
        {
            // No se registran datos clínicos (motivo) en el log, solo identificadores.
            _logger.LogInformation(
                "Push simulado: alerta {IdAlerta} para el médico {IdMedico}, colaborador {IdColaborador}, control el {FechaControl:yyyy-MM-dd}",
                alerta.IdAlerta,
                alerta.IdMedicoResponsable,
                alerta.IdColaborador,
                alerta.FechaControl);

            return Task.FromResult(true);
        }
    }
}
