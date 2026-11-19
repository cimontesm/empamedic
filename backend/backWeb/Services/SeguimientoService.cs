using backWeb.Data;
using backWeb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace backWeb.Services
{
    public class SeguimientoService : ISeguimientoService
    {
        private const int LargoMaximoTexto = 500;

        private readonly AppDbContext _context;
        private readonly INotificacionPushService _notificacionPush;
        private readonly TimeProvider _reloj;
        private readonly SeguimientoOptions _opciones;
        private readonly ILogger<SeguimientoService> _logger;

        public SeguimientoService(
            AppDbContext context,
            INotificacionPushService notificacionPush,
            TimeProvider reloj,
            IOptions<SeguimientoOptions> opciones,
            ILogger<SeguimientoService> logger)
        {
            _context = context;
            _notificacionPush = notificacionPush;
            _reloj = reloj;
            _opciones = opciones.Value;
            _logger = logger;
        }

        private DateTime Ahora => _reloj.GetLocalNow().DateTime;

        /// <summary>Primer día que ya NO entra en la ventana de anticipación.</summary>
        private DateTime LimiteVentana => Ahora.Date.AddDays(_opciones.DiasAnticipacion + 1);

        public async Task<ResultadoCreacion> CrearAsync(
            int idColaborador,
            int idMedicoResponsable,
            DateTime fechaControl,
            string motivo,
            int? idConsultaMedica = null,
            CancellationToken cancellationToken = default)
        {
            var motivoLimpio = motivo?.Trim() ?? string.Empty;

            if (idColaborador <= 0)
            {
                return ResultadoCreacion.Rechazada("Debe indicar el colaborador.");
            }

            if (motivoLimpio.Length == 0 || motivoLimpio.Length > LargoMaximoTexto)
            {
                return ResultadoCreacion.Rechazada($"El motivo es obligatorio y tiene máximo {LargoMaximoTexto} caracteres.");
            }

            if (fechaControl.Date < Ahora.Date)
            {
                return ResultadoCreacion.Rechazada("La fecha de control no puede estar en el pasado.");
            }

            var medico = await _context.Usuarios
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.IdUsuario == idMedicoResponsable, cancellationToken);

            if (medico is null || !medico.Activo)
            {
                return ResultadoCreacion.Rechazada("El médico responsable no existe o está inactivo.");
            }

            if (medico.Rol == RolUsuario.ADMINISTRADOR)
            {
                return ResultadoCreacion.Rechazada("El responsable del control debe ser un médico o enfermero.");
            }

            var alerta = new SeguimientoAlerta
            {
                IdColaborador = idColaborador,
                IdMedicoResponsable = idMedicoResponsable,
                IdConsultaMedica = idConsultaMedica,
                FechaControl = fechaControl,
                Motivo = motivoLimpio,
                Estado = EstadoAlerta.Pendiente,
                FechaCreacion = Ahora
            };

            _context.SeguimientoAlertas.Add(alerta);
            await _context.SaveChangesAsync(cancellationToken);

            return ResultadoCreacion.Ok(alerta);
        }

        public async Task<IReadOnlyList<SeguimientoAlerta>> ObtenerPendientesAsync(
            int idMedico,
            CancellationToken cancellationToken = default)
        {
            var limite = LimiteVentana;

            return await _context.SeguimientoAlertas
                .AsNoTracking()
                .Where(a => a.IdMedicoResponsable == idMedico
                    && a.Estado == EstadoAlerta.Pendiente
                    && a.FechaControl < limite)
                .OrderBy(a => a.FechaControl)
                .ToListAsync(cancellationToken);
        }

        public async Task<ResultadoResolucion> ResolverAsync(
            int idAlerta,
            int idMedico,
            EstadoAlerta nuevoEstado,
            string? observacion,
            CancellationToken cancellationToken = default)
        {
            if (nuevoEstado == EstadoAlerta.Pendiente)
            {
                throw new ArgumentOutOfRangeException(nameof(nuevoEstado), "Una alerta solo se puede marcar como atendida o descartada.");
            }

            var alerta = await _context.SeguimientoAlertas
                .FirstOrDefaultAsync(a => a.IdAlerta == idAlerta, cancellationToken);

            if (alerta is null)
            {
                return ResultadoResolucion.NoEncontrada;
            }

            if (alerta.IdMedicoResponsable != idMedico)
            {
                return ResultadoResolucion.NoAutorizado;
            }

            if (alerta.Estado != EstadoAlerta.Pendiente)
            {
                return ResultadoResolucion.YaResuelta;
            }

            var observacionLimpia = observacion?.Trim();

            alerta.Estado = nuevoEstado;
            alerta.FechaResolucion = Ahora;
            alerta.ObservacionResolucion = string.IsNullOrEmpty(observacionLimpia)
                ? null
                : observacionLimpia[..Math.Min(observacionLimpia.Length, LargoMaximoTexto)];

            await _context.SaveChangesAsync(cancellationToken);

            return ResultadoResolucion.Resuelta;
        }

        public async Task<int> EnviarRecordatoriosAsync(CancellationToken cancellationToken = default)
        {
            var limite = LimiteVentana;

            var porNotificar = await _context.SeguimientoAlertas
                .Where(a => a.Estado == EstadoAlerta.Pendiente
                    && a.FechaNotificacionPush == null
                    && a.FechaControl < limite)
                .OrderBy(a => a.FechaControl)
                .ToListAsync(cancellationToken);

            var enviadas = 0;

            foreach (var alerta in porNotificar)
            {
                try
                {
                    if (await _notificacionPush.EnviarRecordatorioAsync(alerta, cancellationToken))
                    {
                        alerta.FechaNotificacionPush = Ahora;
                        enviadas++;
                    }
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    // Un fallo con una alerta no debe impedir notificar las demás; se reintenta en la siguiente pasada.
                    _logger.LogWarning(ex, "No se pudo enviar el recordatorio de la alerta {IdAlerta}", alerta.IdAlerta);
                }
            }

            if (enviadas > 0)
            {
                await _context.SaveChangesAsync(cancellationToken);
            }

            return enviadas;
        }
    }
}
