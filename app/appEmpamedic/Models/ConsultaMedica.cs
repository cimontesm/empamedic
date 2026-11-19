namespace appEmpamedic.Models
{
    /// <summary>
    /// Consulta médica registrada en el dispensario.
    /// DTO del lado de la app: debe coincidir con lo que devuelva la API (integrante 3).
    /// </summary>
    public class ConsultaMedica
    {
        public int Id { get; set; }
        public int ColaboradorId { get; set; }
        public string NombreColaborador { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public string MotivoConsulta { get; set; } = string.Empty;
        public string Diagnostico { get; set; } = string.Empty;
        public string? Tratamiento { get; set; }
        public DateTime? FechaProximoControl { get; set; }
    }
}
