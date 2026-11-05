namespace appEmpamedic.Models
{
    /// <summary>
    /// Colaborador de EMPACRECI atendido en el dispensario (paciente).
    /// DTO del lado de la app: debe coincidir con lo que devuelva la API (integrante 2).
    /// </summary>
    public class Colaborador
    {
        public int Id { get; set; }
        public string Cedula { get; set; } = string.Empty;
        public string Nombres { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string Cargo { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public DateTime FechaNacimiento { get; set; }
        public string? TipoSangre { get; set; }
        public string? Alergias { get; set; }
        public string? Antecedentes { get; set; }

        public string NombreCompleto => $"{Nombres} {Apellidos}";
    }
}
