using System.Net;
using System.Net.Http.Json;

namespace appEmpamedic.Services
{
    public record ResultadoLogin(bool Exito, string? Mensaje);

    /// <summary>
    /// Sesión del usuario en la app. Inicia sesión contra POST /api/Auth/login (RF01, integrante 1).
    /// Con <see cref="ApiConfig.UsarDatosSimulados"/> acepta cualquier usuario y asigna el médico 1.
    /// TODO: guardar el token cuando la API lo devuelva (JWT) y enviarlo en cada llamada.
    /// </summary>
    public class SesionService
    {
        private readonly HttpClient _http;

        public SesionService(HttpClient http)
        {
            _http = http;
        }

        public int? UsuarioId { get; private set; }
        public string NombreUsuario { get; private set; } = string.Empty;
        public string Rol { get; private set; } = string.Empty;
        public bool EsMedico { get; private set; }

        public bool HaySesion => UsuarioId is not null;

        public async Task<ResultadoLogin> IniciarSesionAsync(string usuario, string clave)
        {
            if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(clave))
                return new ResultadoLogin(false, "Ingrese usuario y contraseña.");

            if (ApiConfig.UsarDatosSimulados)
            {
                AbrirSesion(1, usuario.Trim(), "MEDICO", esMedico: true);
                return new ResultadoLogin(true, null);
            }

            try
            {
                using var respuesta = await _http.PostAsJsonAsync(
                    "api/Auth/login",
                    new { Username = usuario.Trim(), Password = clave });

                if (respuesta.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.BadRequest)
                {
                    var error = await LeerAsync<RespuestaError>(respuesta);
                    return new ResultadoLogin(false, error?.Mensaje ?? "Credenciales incorrectas.");
                }

                respuesta.EnsureSuccessStatusCode();

                var datos = await LeerAsync<RespuestaLogin>(respuesta);
                if (datos is null)
                    return new ResultadoLogin(false, "El servidor devolvió una respuesta inesperada.");

                AbrirSesion(datos.IdUsuario, datos.Nombre, datos.Rol, datos.EsMedico);
                return new ResultadoLogin(true, null);
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
            {
                return new ResultadoLogin(false, $"No se pudo conectar con el servidor ({ApiConfig.UrlBase}). Verifique que la API esté encendida.");
            }
        }

        public void CerrarSesion()
        {
            UsuarioId = null;
            NombreUsuario = string.Empty;
            Rol = string.Empty;
            EsMedico = false;
        }

        private void AbrirSesion(int id, string nombre, string rol, bool esMedico)
        {
            UsuarioId = id;
            NombreUsuario = nombre;
            Rol = rol;
            EsMedico = esMedico;
        }

        private static async Task<T?> LeerAsync<T>(HttpResponseMessage respuesta)
        {
            try
            {
                return await respuesta.Content.ReadFromJsonAsync<T>();
            }
            catch (System.Text.Json.JsonException)
            {
                return default;
            }
        }

        private sealed record RespuestaLogin(int IdUsuario, string Nombre, string Rol, bool EsMedico);

        private sealed record RespuestaError(string? Mensaje);
    }
}
