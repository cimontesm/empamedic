using frontWeb.Models;
using frontWeb.Services;
using Microsoft.AspNetCore.Mvc;

namespace frontWeb.Controllers;

// TODO (integración con Auth): agregar [Authorize(Roles = "Medico,Administrador")]
// cuando el login/roles del integrante 1 esté listo (RF09, último criterio de aceptación).
public class ReportesController : Controller
{
    private readonly IReportesApiClient _reportesApi;
    private readonly ILogger<ReportesController> _logger;

    public ReportesController(IReportesApiClient reportesApi, ILogger<ReportesController> logger)
    {
        _reportesApi = reportesApi;
        _logger = logger;
    }

    // GET: /Reportes?fechaInicio=2026-11-01&fechaFin=2026-11-12
    public async Task<IActionResult> Index(DateTime? fechaInicio, DateTime? fechaFin)
    {
        // Por defecto: del primer día del mes actual a hoy (igual al ejemplo del prototipo).
        var hasta = fechaFin ?? DateTime.Today;
        var desde = fechaInicio ?? new DateTime(hasta.Year, hasta.Month, 1);

        var modelo = new ReportesPageViewModel
        {
            FechaInicio = desde,
            FechaFin = hasta
        };

        try
        {
            // RNF05 / HU7: si el backend tarda más de 5s, se corta con un mensaje
            // amigable en vez de dejar la pantalla colgada.
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            modelo.Resumen = await _reportesApi.ObtenerResumenAsync(desde, hasta, cts.Token);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Timeout al consultar el API de reportes ({Desde} - {Hasta})", desde, hasta);
            modelo.MensajeError = "El servidor tardó demasiado en responder. Intenta de nuevo en unos segundos.";
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error al consultar el API de reportes");
            modelo.MensajeError = "No se pudo conectar con el servidor de reportes. Verifica que el backend esté corriendo.";
        }

        return View(modelo);
    }

    [HttpGet]
    public async Task<IActionResult> ExportarExcel(DateTime fechaInicio, DateTime fechaFin)
    {
        try
        {
            var (contenido, nombreArchivo) = await _reportesApi.ExportarExcelAsync(fechaInicio, fechaFin);
            return File(contenido, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nombreArchivo);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error al exportar reporte Excel");
            TempData["MensajeError"] = "No se pudo generar el archivo Excel. Intenta de nuevo.";
            return RedirectToAction(nameof(Index), new { fechaInicio, fechaFin });
        }
    }

    [HttpGet]
    public async Task<IActionResult> ExportarCsv(DateTime fechaInicio, DateTime fechaFin)
    {
        try
        {
            var (contenido, nombreArchivo) = await _reportesApi.ExportarCsvAsync(fechaInicio, fechaFin);
            return File(contenido, "text/csv", nombreArchivo);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error al exportar reporte CSV");
            TempData["MensajeError"] = "No se pudo generar el archivo CSV. Intenta de nuevo.";
            return RedirectToAction(nameof(Index), new { fechaInicio, fechaFin });
        }
    }
}
