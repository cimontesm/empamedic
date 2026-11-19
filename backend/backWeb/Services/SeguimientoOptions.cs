namespace backWeb.Services
{
    /// <summary>
    /// Configuración de las alertas de seguimiento. Se lee de la sección "Seguimiento" de appsettings.json.
    /// </summary>
    public class SeguimientoOptions
    {
        public const string Seccion = "Seguimiento";

        /// <summary>Días antes del control en que la alerta empieza a mostrarse y a notificarse.</summary>
        public int DiasAnticipacion { get; set; } = 3;

        /// <summary>Cada cuántos minutos se buscan recordatorios para enviar por push.</summary>
        public int IntervaloRecordatoriosMinutos { get; set; } = 15;

        /// <summary>Permite apagar el envío automático (por ejemplo, en pruebas).</summary>
        public bool RecordatoriosHabilitados { get; set; } = true;
    }
}
