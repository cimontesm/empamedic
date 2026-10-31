
using backWeb.Services;
using Microsoft.AspNetCore.Mvc;

namespace backWeb.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet("resumen")]
    public IActionResult ObtenerResumen(
        [FromQuery] DateTime fechaInicio,
        [FromQuery] DateTime fechaFin)
    {
        if (fechaInicio == default || fechaFin == default)
        {
            return BadRequest(new
            {
                mensaje = "Debe proporcionar una fecha de inicio y una fecha de fin."
            });
        }

        if (fechaInicio.Date > fechaFin.Date)
        {
            return BadRequest(new
            {
                mensaje = "La fecha de inicio no puede ser posterior a la fecha de fin."
            });
        }

        var reporte = _reportService.GenerarReporte(
            fechaInicio,
            fechaFin
        );

        return Ok(reporte);
    }
}
