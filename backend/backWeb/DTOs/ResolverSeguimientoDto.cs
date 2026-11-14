using System.ComponentModel.DataAnnotations;

namespace backWeb.DTOs
{
    /// <summary>Datos para marcar una alerta como atendida o descartada.</summary>
    public class ResolverSeguimientoDto
    {
        /// <summary>
        /// Médico que resuelve la alerta.
        /// TODO: tomarlo del token cuando la API tenga autenticación con JWT (integrante 1);
        /// mientras tanto viaja en el cuerpo y no es una garantía de seguridad.
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "Debe indicar el médico.")]
        public int IdMedico { get; set; }

        [StringLength(500)]
        public string? Observacion { get; set; }
    }
}
