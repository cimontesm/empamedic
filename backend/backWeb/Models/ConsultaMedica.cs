namespace backWeb.Models
{
    public class ConsultaMedica
    {
        public int Id { get; set; }
        public string CedulaPaciente { get; set; } = string.Empty;
        public string NombrePaciente { get; set; } = string.Empty;
        public DateTime FechaConsulta { get; set; }
        public string MotivoConsulta { get; set; } = string.Empty;
        public string? Sintomas { get; set; }
        public string Diagnostico { get; set; } = string.Empty;
        public string? Observaciones { get; set; }
        public string? PresionArterial { get; set; }
        public decimal? Temperatura { get; set; }
        public string Tratamiento { get; set; } = string.Empty;
        public string MedicinaRecetada { get; set; } = string.Empty;
        public int DiasReposo { get; set; }
    }
}