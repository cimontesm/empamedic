namespace appEmpamedic.Services
{
    /// <summary>
    /// Dirección del backend (backWeb) y modo de datos de la app.
    /// </summary>
    public static class ApiConfig
    {
        /// <summary>
        /// true: la app funciona sin backend, con datos ficticios y login simulado (útil para demos).
        /// false: login y alertas reales contra la API.
        /// </summary>
        public static readonly bool UsarDatosSimulados = false;

        /// <summary>Puerto del perfil "http" de backWeb (Properties/launchSettings.json).</summary>
        private const int PuertoApi = 5164;

        /// <summary>
        /// En el emulador de Android, "localhost" es el propio emulador: la PC se alcanza con 10.0.2.2.
        /// En un celular físico hay que poner la IP de la PC en la red local (ej. http://192.168.1.20:5164)
        /// y abrir el puerto en el firewall de Windows.
        /// </summary>
        public static Uri UrlBase => DeviceInfo.Platform == DevicePlatform.Android
            ? new Uri($"http://10.0.2.2:{PuertoApi}/")
            : new Uri($"http://localhost:{PuertoApi}/");

        public static readonly TimeSpan TiempoEspera = TimeSpan.FromSeconds(15);
    }
}
