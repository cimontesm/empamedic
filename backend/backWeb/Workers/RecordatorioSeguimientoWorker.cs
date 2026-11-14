using backWeb.Services;
using Microsoft.Extensions.Options;

namespace backWeb.Workers
{
    /// <summary>
    /// Proceso en segundo plano que revisa periódicamente las alertas próximas
    /// y envía las notificaciones push a los médicos responsables (RF08).
    /// </summary>
    public class RecordatorioSeguimientoWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly SeguimientoOptions _opciones;
        private readonly ILogger<RecordatorioSeguimientoWorker> _logger;

        public RecordatorioSeguimientoWorker(
            IServiceScopeFactory scopeFactory,
            IOptions<SeguimientoOptions> opciones,
            ILogger<RecordatorioSeguimientoWorker> logger)
        {
            _scopeFactory = scopeFactory;
            _opciones = opciones.Value;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!_opciones.RecordatoriosHabilitados)
            {
                _logger.LogInformation("El envío automático de recordatorios está deshabilitado.");
                return;
            }

            var intervalo = TimeSpan.FromMinutes(Math.Max(1, _opciones.IntervaloRecordatoriosMinutos));
            using var temporizador = new PeriodicTimer(intervalo);

            do
            {
                try
                {
                    // El DbContext es "scoped": se crea un scope nuevo en cada pasada.
                    using var scope = _scopeFactory.CreateScope();
                    var servicio = scope.ServiceProvider.GetRequiredService<ISeguimientoService>();

                    var enviadas = await servicio.EnviarRecordatoriosAsync(stoppingToken);
                    if (enviadas > 0)
                    {
                        _logger.LogInformation("Se enviaron {Cantidad} recordatorios de control.", enviadas);
                    }
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    // Sin base de datos disponible (o cualquier otro error) se registra y se intenta en la próxima pasada,
                    // sin detener la API.
                    _logger.LogWarning(ex, "Falló la revisión de recordatorios de control.");
                }
            }
            while (await temporizador.WaitForNextTickAsync(stoppingToken));
        }
    }
}
