using System.Net.Http.Json;

namespace appEmpamedic.Services
{
    /// <summary>
    /// Implementación contra el backend real (backWeb).
    /// Hoy solo las alertas de seguimiento (RF08) tienen endpoints en main; pacientes, consultas e
    /// inventario siguen viniendo de <see cref="MockEmpamedicApi"/> hasta que se integren
    /// los endpoints de los integrantes 2 y 3.
    /// </summary>
    public class HttpEmpamedicApi : IEmpamedicApi
    {
        private readonly HttpClient _http;
        private readonly SesionService _sesion;
        private readonly MockEmpamedicApi _simulado;

        public HttpEmpamedicApi(HttpClient http, SesionService sesion, MockEmpamedicApi simulado)
        {
            _http = http;
            _sesion = sesion;
            _simulado = simulado;
        }

        // ---------- Pendientes de backend (datos simulados) ----------

        public Task<List<Colaborador>> ObtenerColaboradoresAsync() => _simulado.ObtenerColaboradoresAsync();

        public Task<Colaborador?> ObtenerColaboradorAsync(int id) => _simulado.ObtenerColaboradorAsync(id);

        public Task<List<ConsultaMedica>> ObtenerConsultasAsync(int? colaboradorId = null) => _simulado.ObtenerConsultasAsync(colaboradorId);

        public Task<List<Medicamento>> ObtenerMedicamentosAsync() => _simulado.ObtenerMedicamentosAsync();

        // ---------- Seguimiento (RF08): API real ----------

        public async Task<List<SeguimientoAlerta>> ObtenerAlertasPendientesAsync(int medicoId)
        {
            var alertas = await EjecutarAsync(() =>
                _http.GetFromJsonAsync<List<AlertaApi>>($"api/seguimiento/pendientes?idMedico={medicoId}"));

            var resultado = new List<SeguimientoAlerta>();
            foreach (var a in alertas ?? [])
            {
                resultado.Add(new SeguimientoAlerta
                {
                    Id = a.IdAlerta,
                    ColaboradorId = a.IdColaborador,
                    // TODO: usar el nombre real cuando exista el endpoint de colaboradores (integrante 2).
                    NombreColaborador = (await _simulado.ObtenerColaboradorAsync(a.IdColaborador))?.NombreCompleto
                        ?? $"Colaborador #{a.IdColaborador}",
                    MedicoResponsableId = a.IdMedicoResponsable,
                    ConsultaMedicaId = a.IdConsultaMedica,
                    FechaControl = a.FechaControl,
                    Motivo = a.Motivo,
                    Estado = Enum.TryParse<EstadoAlerta>(a.Estado, out var estado) ? estado : EstadoAlerta.Pendiente,
                    FechaResolucion = a.FechaResolucion
                });
            }

            return resultado;
        }

        public async Task ActualizarEstadoAlertaAsync(int alertaId, EstadoAlerta estado)
        {
            if (_sesion.UsuarioId is not int medicoId)
                throw new InvalidOperationException("La sesión expiró. Vuelva a iniciar sesión.");

            var accion = estado switch
            {
                EstadoAlerta.Atendida => "atender",
                EstadoAlerta.Descartada => "descartar",
                _ => throw new ArgumentOutOfRangeException(nameof(estado), "Solo se puede atender o descartar una alerta.")
            };

            using var respuesta = await EjecutarAsync(() =>
                _http.PutAsJsonAsync($"api/seguimiento/{alertaId}/{accion}", new { IdMedico = medicoId }));

            if (!respuesta.IsSuccessStatusCode)
            {
                var error = await LeerErrorAsync(respuesta);
                throw new InvalidOperationException(error ?? $"No se pudo actualizar la alerta ({(int)respuesta.StatusCode}).");
            }
        }

        /// <summary>Convierte los errores de red en un mensaje entendible para el usuario.</summary>
        private static async Task<T> EjecutarAsync<T>(Func<Task<T>> llamada)
        {
            try
            {
                return await llamada();
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
            {
                throw new InvalidOperationException(
                    $"No se pudo conectar con el servidor ({ApiConfig.UrlBase}). Verifique que la API esté encendida.", ex);
            }
        }

        private static async Task<string?> LeerErrorAsync(HttpResponseMessage respuesta)
        {
            try
            {
                return (await respuesta.Content.ReadFromJsonAsync<RespuestaError>())?.Mensaje;
            }
            catch (System.Text.Json.JsonException)
            {
                return null;
            }
        }

        /// <summary>Forma de SeguimientoAlertaDto del backend.</summary>
        private sealed record AlertaApi(
            int IdAlerta,
            int IdColaborador,
            int IdMedicoResponsable,
            int? IdConsultaMedica,
            DateTime FechaControl,
            string Motivo,
            string Estado,
            bool Vencida,
            DateTime? FechaResolucion);

        private sealed record RespuestaError(string? Mensaje);
    }
}
