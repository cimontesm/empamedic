namespace appEmpamedic.Services
{
    /// <summary>
    /// Sesión del usuario en la app.
    /// SIMULADA: acepta cualquier usuario y contraseña no vacíos y asigna el médico 1.
    /// Se reemplaza por el login real (JWT) cuando el integrante 1 publique el endpoint.
    /// </summary>
    public class SesionService
    {
        public int? UsuarioId { get; private set; }
        public string NombreUsuario { get; private set; } = string.Empty;

        public bool HaySesion => UsuarioId is not null;

        public Task<bool> IniciarSesionAsync(string usuario, string clave)
        {
            if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(clave))
                return Task.FromResult(false);

            UsuarioId = 1;
            NombreUsuario = usuario.Trim();
            return Task.FromResult(true);
        }

        public void CerrarSesion()
        {
            UsuarioId = null;
            NombreUsuario = string.Empty;
        }
    }
}
