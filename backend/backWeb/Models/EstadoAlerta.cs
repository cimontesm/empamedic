namespace backWeb.Models
{
    /// <summary>
    /// Estado de una alerta de seguimiento (RF08).
    /// </summary>
    public enum EstadoAlerta
    {
        /// <summary>El control todavía no se realiza.</summary>
        Pendiente,

        /// <summary>El médico realizó el control o se comunicó con el paciente.</summary>
        Atendida,

        /// <summary>El médico decidió que el control ya no es necesario.</summary>
        Descartada
    }
}
