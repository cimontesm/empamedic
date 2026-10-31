
namespace backWeb.Models;

public class ReporteResumenDto
{
    public DateTime FechaInicio { get; set; }

    public DateTime FechaFin { get; set; }

    public int TotalColaboradoresAtendidos { get; set; }

    public int TotalAtenciones { get; set; }

    public int TotalDiasReposo { get; set; }

    public List<DiagnosticoFrecuenteDto> DiagnosticosFrecuentes { get; set; } = new();

    public List<ConsultaMedica> DetalleAtenciones { get; set; } = new();
}
