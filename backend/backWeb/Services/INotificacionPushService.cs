using backWeb.Models;

namespace backWeb.Services
{
    /// <summary>
    /// Envía notificaciones push al celular del médico (RF08).
    /// </summary>
    public interface INotificacionPushService
    {
        /// <summary>
        /// Envía el recordatorio de una alerta a su médico responsable.
        /// Devuelve false si no se pudo enviar, para reintentarlo en la siguiente pasada.
        /// </summary>
        Task<bool> EnviarRecordatorioAsync(SeguimientoAlerta alerta, CancellationToken cancellationToken = default);
    }
}
