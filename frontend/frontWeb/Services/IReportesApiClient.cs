using frontWeb.Models;

namespace frontWeb.Services;

public interface IReportesApiClient
{
    Task<ReporteResumenViewModel?> ObtenerResumenAsync(DateTime fechaInicio, DateTime fechaFin, CancellationToken ct = default);
    Task<(byte[] Contenido, string NombreArchivo)> ExportarExcelAsync(DateTime fechaInicio, DateTime fechaFin, CancellationToken ct = default);
    Task<(byte[] Contenido, string NombreArchivo)> ExportarCsvAsync(DateTime fechaInicio, DateTime fechaFin, CancellationToken ct = default);
}
