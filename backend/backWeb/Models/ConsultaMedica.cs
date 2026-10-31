namespace backWeb.Models
{
    public class ConsultaMedica
    {
        public int Id { get; set; }
        public string PacienteId { get; set; } = string.Empty;
        public string NombrePaciente { get; set; } = string.Empty;
        public DateTime FechaConsulta { get; set; }
        public string Diagnostico { get; set; } = string.Empty;
        public string Tratamiento { get; set; } = string.Empty;
        public int DiasReposo { get; set; }
        
    }
}