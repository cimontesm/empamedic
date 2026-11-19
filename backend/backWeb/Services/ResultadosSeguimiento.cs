using backWeb.Models;

namespace backWeb.Services
{
    /// <summary>Resultado de crear una alerta: la alerta creada o el motivo del rechazo.</summary>
    public record ResultadoCreacion(SeguimientoAlerta? Alerta, string? Error)
    {
        public bool Exito => Alerta is not null;

        public static ResultadoCreacion Ok(SeguimientoAlerta alerta) => new(alerta, null);

        public static ResultadoCreacion Rechazada(string error) => new(null, error);
    }

    /// <summary>Resultado de marcar una alerta como atendida o descartada.</summary>
    public enum ResultadoResolucion
    {
        Resuelta,
        NoEncontrada,

        /// <summary>Quien intenta resolverla no es el médico responsable.</summary>
        NoAutorizado,

        /// <summary>La alerta ya estaba atendida o descartada.</summary>
        YaResuelta
    }
}
