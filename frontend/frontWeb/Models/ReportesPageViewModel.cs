namespace frontWeb.Models;

public class ReportesPageViewModel
{
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public ReporteResumenViewModel? Resumen { get; set; }

    // Se llena si el backend no responde a tiempo o está caído, para mostrar
    // un mensaje amigable en vez de romper la página (RNF05).
    public string? MensajeError { get; set; }
}
