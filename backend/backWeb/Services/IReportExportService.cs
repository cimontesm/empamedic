namespace backWeb.Services;

public interface IReportExportService
{
    byte[] GenerarExcel(DateTime fechaInicio, DateTime fechaFin);
}
