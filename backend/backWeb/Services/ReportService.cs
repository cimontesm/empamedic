
using backWeb.Data;
using backWeb.Models;

namespace backWeb.Services;

public class ReportService : IReportService
{
    public ReporteResumenDto GenerarReporte(
        DateTime fechaInicio,
        DateTime fechaFin)
    {
        // Filtrar las consultas según el rango de fechas.
        var consultas = ConsultaMedicaData.Consultas
            .Where(c =>
                c.FechaConsulta.Date >= fechaInicio.Date &&
                c.FechaConsulta.Date <= fechaFin.Date)
            .OrderBy(c => c.FechaConsulta)
            .ToList();

        // Calcular los diagnósticos más frecuentes.
        var diagnosticos = consultas
            .GroupBy(c => c.Diagnostico)
            .Select(g => new DiagnosticoFrecuenteDto
            {
                Diagnostico = g.Key,
                Cantidad = g.Count()
            })
            .OrderByDescending(d => d.Cantidad)
            .ToList();

        return new ReporteResumenDto
        {
            FechaInicio = fechaInicio.Date,
            FechaFin = fechaFin.Date,

            TotalColaboradoresAtendidos = consultas
                .Select(c => c.PacienteId)
                .Distinct()
                .Count(),

            TotalAtenciones = consultas.Count,

            TotalDiasReposo = consultas.Sum(c => c.DiasReposo),

            DiagnosticosFrecuentes = diagnosticos,

            DetalleAtenciones = consultas
        };
    }
}
