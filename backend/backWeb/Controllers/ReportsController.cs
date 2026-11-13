
using backWeb.Services;
using Microsoft.AspNetCore.Mvc;

namespace backWeb.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    private readonly IReportExportService _reportExportService;

    public ReportsController(IReportService reportService, IReportExportService reportExportService)
    {
        _reportService = reportService;
        _reportExportService = reportExportService;
    }

    [HttpGet("resumen")]
    public IActionResult ObtenerResumen(
        [FromQuery] DateTime fechaInicio,
        [FromQuery] DateTime fechaFin)
    {
        var error = ValidarFechas(fechaInicio, fechaFin);
        if (error is not null) return error;

        var reporte = _reportService.GenerarReporte(
            fechaInicio,
            fechaFin
        );

        return Ok(reporte);
    }

    [HttpGet("exportar/xlsx")]
    public IActionResult ExportarExcel(
        [FromQuery] DateTime fechaInicio,
        [FromQuery] DateTime fechaFin)
    {
        var error = ValidarFechas(fechaInicio, fechaFin);
        if (error is not null) return error;

        var archivo = _reportExportService.GenerarExcel(fechaInicio, fechaFin);
        var nombreArchivo = $"dispensario-empacreci_{fechaInicio:yyyy-MM-dd}_a_{fechaFin:yyyy-MM-dd}.xlsx";

        return File(archivo, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nombreArchivo);
    }

    private IActionResult? ValidarFechas(DateTime fechaInicio, DateTime fechaFin)
    {
        if (fechaInicio == default || fechaFin == default)
        {
            return BadRequest(new { mensaje = "Debe proporcionar una fecha de inicio y una fecha de fin." });
        }

        if (fechaInicio.Date > fechaFin.Date)
        {
            return BadRequest(new { mensaje = "La fecha de inicio no puede ser posterior a la fecha de fin." });
        }

        return null;
    }
}
