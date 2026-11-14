using backWeb.DTOs;
using backWeb.Models;
using backWeb.Services;
using Microsoft.AspNetCore.Mvc;

namespace backWeb.Controllers
{
    /// <summary>
    /// Alertas y recordatorios de control médico (RF08).
    /// TODO: cuando exista autenticación con JWT (integrante 1), proteger con [Authorize]
    /// y tomar el id del médico del token en lugar de recibirlo como parámetro.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class SeguimientoController : ControllerBase
    {
        private readonly ISeguimientoService _servicio;
        private readonly TimeProvider _reloj;

        public SeguimientoController(ISeguimientoService servicio, TimeProvider reloj)
        {
            _servicio = servicio;
            _reloj = reloj;
        }

        private DateTime Hoy => _reloj.GetLocalNow().Date;

        /// <summary>Programa un control de seguimiento para un colaborador.</summary>
        [HttpPost]
        [ProducesResponseType<SeguimientoAlertaDto>(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Crear([FromBody] CrearSeguimientoDto dto, CancellationToken cancellationToken)
        {
            var resultado = await _servicio.CrearAsync(
                dto.IdColaborador,
                dto.IdMedicoResponsable,
                dto.FechaControl,
                dto.Motivo,
                dto.IdConsultaMedica,
                cancellationToken);

            if (resultado.Alerta is null)
            {
                return BadRequest(new { mensaje = resultado.Error });
            }

            return StatusCode(StatusCodes.Status201Created, SeguimientoAlertaDto.Desde(resultado.Alerta, Hoy));
        }

        /// <summary>Alertas pendientes (vencidas y próximas) del médico indicado.</summary>
        [HttpGet("pendientes")]
        [ProducesResponseType<IEnumerable<SeguimientoAlertaDto>>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ObtenerPendientes([FromQuery] int idMedico, CancellationToken cancellationToken)
        {
            if (idMedico <= 0)
            {
                return BadRequest(new { mensaje = "Debe indicar el médico." });
            }

            var alertas = await _servicio.ObtenerPendientesAsync(idMedico, cancellationToken);
            var hoy = Hoy;

            return Ok(alertas.Select(a => SeguimientoAlertaDto.Desde(a, hoy)));
        }

        /// <summary>Marca la alerta como atendida (el control se realizó).</summary>
        [HttpPut("{idAlerta:int}/atender")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public Task<IActionResult> Atender(int idAlerta, [FromBody] ResolverSeguimientoDto dto, CancellationToken cancellationToken)
            => Resolver(idAlerta, dto, EstadoAlerta.Atendida, cancellationToken);

        /// <summary>Marca la alerta como descartada (el control ya no es necesario).</summary>
        [HttpPut("{idAlerta:int}/descartar")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public Task<IActionResult> Descartar(int idAlerta, [FromBody] ResolverSeguimientoDto dto, CancellationToken cancellationToken)
            => Resolver(idAlerta, dto, EstadoAlerta.Descartada, cancellationToken);

        private async Task<IActionResult> Resolver(
            int idAlerta,
            ResolverSeguimientoDto dto,
            EstadoAlerta nuevoEstado,
            CancellationToken cancellationToken)
        {
            var resultado = await _servicio.ResolverAsync(idAlerta, dto.IdMedico, nuevoEstado, dto.Observacion, cancellationToken);

            return resultado switch
            {
                ResultadoResolucion.Resuelta => NoContent(),
                ResultadoResolucion.NoEncontrada => NotFound(new { mensaje = "La alerta no existe." }),
                ResultadoResolucion.NoAutorizado => StatusCode(
                    StatusCodes.Status403Forbidden,
                    new { mensaje = "Solo el médico responsable puede resolver esta alerta." }),
                ResultadoResolucion.YaResuelta => Conflict(new { mensaje = "La alerta ya fue atendida o descartada." }),
                _ => throw new InvalidOperationException($"Resultado no contemplado: {resultado}")
            };
        }
    }
}
