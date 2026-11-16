using System.Net.Http.Json;
using frontWeb.Models;

namespace frontWeb.Services;

// Único punto por donde el frontend toca los datos: todo pasa por HTTP hacia el
// backend (backWeb). No hay acceso directo a ninguna base de datos desde aquí.
public class ReportesApiClient : IReportesApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ReportesApiClient> _logger;

    public ReportesApiClient(HttpClient httpClient, ILogger<ReportesApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<ReporteResumenViewModel?> ObtenerResumenAsync(DateTime fechaInicio, DateTime fechaFin, CancellationToken ct = default)
    {
        var url = $"api/reports/resumen?fechaInicio={fechaInicio:yyyy-MM-dd}&fechaFin={fechaFin:yyyy-MM-dd}";
        using var respuesta = await _httpClient.GetAsync(url, ct);
        respuesta.EnsureSuccessStatusCode();
        return await respuesta.Content.ReadFromJsonAsync<ReporteResumenViewModel>(cancellationToken: ct);
    }

    public Task<(byte[] Contenido, string NombreArchivo)> ExportarExcelAsync(DateTime fechaInicio, DateTime fechaFin, CancellationToken ct = default)
        => DescargarArchivoAsync($"api/reports/exportar/xlsx?fechaInicio={fechaInicio:yyyy-MM-dd}&fechaFin={fechaFin:yyyy-MM-dd}", "reporte.xlsx", ct);

    public Task<(byte[] Contenido, string NombreArchivo)> ExportarCsvAsync(DateTime fechaInicio, DateTime fechaFin, CancellationToken ct = default)
        => DescargarArchivoAsync($"api/reports/exportar/csv?fechaInicio={fechaInicio:yyyy-MM-dd}&fechaFin={fechaFin:yyyy-MM-dd}", "reporte.csv", ct);

    private async Task<(byte[], string)> DescargarArchivoAsync(string url, string nombrePorDefecto, CancellationToken ct)
    {
        using var respuesta = await _httpClient.GetAsync(url, ct);
        respuesta.EnsureSuccessStatusCode();

        var contenido = await respuesta.Content.ReadAsByteArrayAsync(ct);
        var nombreArchivo = respuesta.Content.Headers.ContentDisposition?.FileNameStar
            ?? respuesta.Content.Headers.ContentDisposition?.FileName
            ?? nombrePorDefecto;

        return (contenido, nombreArchivo.Trim('"'));
    }
}
