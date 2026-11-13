using System.Globalization;
using ClosedXML.Excel;

namespace backWeb.Services;

public class ReportExportService : IReportExportService
{
    private readonly IReportService _reportService;

    public ReportExportService(IReportService reportService)
    {
        _reportService = reportService;
    }

    public byte[] GenerarExcel(DateTime fechaInicio, DateTime fechaFin)
    {
        var reporte = _reportService.GenerarReporte(fechaInicio, fechaFin);

        using var workbook = new XLWorkbook();
        var hoja = workbook.Worksheets.Add("Atenciones");

        var encabezados = new[]
        {
            "Fecha", "Cédula", "Colaborador", "Motivo", "Síntomas",
            "Presión arterial", "Temperatura", "Diagnóstico",
            "Tratamiento", "Medicina recetada", "Observaciones", "Días de reposo"
        };

        for (var i = 0; i < encabezados.Length; i++)
        {
            hoja.Cell(1, i + 1).Value = encabezados[i];
            hoja.Cell(1, i + 1).Style.Font.Bold = true;
        }

        var fila = 2;
        foreach (var c in reporte.DetalleAtenciones)
        {
            hoja.Cell(fila, 1).Value = c.FechaConsulta;
            hoja.Cell(fila, 1).Style.DateFormat.Format = "dd/mm/yyyy hh:mm";
            hoja.Cell(fila, 2).Value = c.CedulaPaciente;
            hoja.Cell(fila, 3).Value = c.NombrePaciente;
            hoja.Cell(fila, 4).Value = c.MotivoConsulta;
            hoja.Cell(fila, 5).Value = c.Sintomas ?? string.Empty;
            hoja.Cell(fila, 6).Value = c.PresionArterial ?? string.Empty;
            hoja.Cell(fila, 7).Value = c.Temperatura.HasValue ? c.Temperatura.Value.ToString(CultureInfo.InvariantCulture) : string.Empty;
            hoja.Cell(fila, 8).Value = c.Diagnostico;
            hoja.Cell(fila, 9).Value = c.Tratamiento;
            hoja.Cell(fila, 10).Value = c.MedicinaRecetada;
            hoja.Cell(fila, 11).Value = c.Observaciones ?? string.Empty;
            hoja.Cell(fila, 12).Value = c.DiasReposo;
            fila++;
        }

        hoja.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
    
}
