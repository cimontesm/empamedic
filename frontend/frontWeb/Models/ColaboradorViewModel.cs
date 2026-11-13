using System.ComponentModel.DataAnnotations;

namespace frontWeb.Models
{
    public class ColaboradorViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "La cédula es obligatoria")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "La cédula debe tener 10 dígitos")]
        [Display(Name = "Cédula")]
        public string Cedula { get; set; } = string.Empty;

        [Required(ErrorMessage = "Los nombres son obligatorios")]
        [Display(Name = "Nombres")]
        public string Nombres { get; set; } = string.Empty;

        [Required(ErrorMessage = "Los apellidos son obligatorios")]
        [Display(Name = "Apellidos")]
        public string Apellidos { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Nacimiento")]
        public DateTime FechaNacimiento { get; set; }

        [Required(ErrorMessage = "El tipo de sangre es obligatorio")]
        [Display(Name = "Tipo de Sangre")]
        public string TipoSangre { get; set; } = string.Empty;

        [Display(Name = "Alergias conocidas")]
        public string? Alergias { get; set; }

        [Display(Name = "Teléfono de contacto")]
        public string? Telefono { get; set; }

        [Display(Name = "Cargo / Área")]
        public string? Cargo { get; set; }

        [Display(Name = "Activo")]
        public bool Activo { get; set; } = true;
    }
}
