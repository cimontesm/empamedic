using System.ComponentModel.DataAnnotations;

namespace backWeb.DTOs
{
    /// <summary>Datos para programar un control de seguimiento.</summary>
    public class CrearSeguimientoDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "Debe indicar el colaborador.")]
        public int IdColaborador { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debe indicar el médico responsable.")]
        public int IdMedicoResponsable { get; set; }

        public int? IdConsultaMedica { get; set; }

        [Required]
        public DateTime FechaControl { get; set; }

        [Required(ErrorMessage = "El motivo es obligatorio.")]
        [StringLength(500)]
        public string Motivo { get; set; } = string.Empty;
    }
}
