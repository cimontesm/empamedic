using backWeb.Models;

namespace backWeb.Services;

public interface IReportService
{
    ReporteResumenDto GenerarReporte(DateTime fechaInicio, DateTime fechaFin);
}
