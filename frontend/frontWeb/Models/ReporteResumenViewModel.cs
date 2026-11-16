namespace frontWeb.Models;

public class ReporteResumenViewModel
{
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public int TotalColaboradoresAtendidos { get; set; }
    public int TotalAtenciones { get; set; }
    public int AtencionesConReposo { get; set; }
    public int TotalDiasReposo { get; set; }
    public int MedicamentosEntregados { get; set; }
    public List<DiagnosticoFrecuenteViewModel> DiagnosticosFrecuentes { get; set; } = new();
    public List<ConsultaMedicaViewModel> DetalleAtenciones { get; set; } = new();
}

public class DiagnosticoFrecuenteViewModel
{
    public string Diagnostico { get; set; } = string.Empty;
    public int Cantidad { get; set; }
}
