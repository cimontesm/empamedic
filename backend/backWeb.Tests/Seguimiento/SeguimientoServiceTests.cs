using backWeb.Data;
using backWeb.Models;
using backWeb.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace backWeb.Tests.Seguimiento
{
    /// <summary>
    /// Pruebas de RF08 con una base SQLite en memoria (no necesitan SQL Server).
    /// "Hoy" está fijo en el 13/11/2026 a las 09:00 (hora de Ecuador) y la anticipación es de 3 días.
    /// </summary>
    public sealed class SeguimientoServiceTests : IDisposable
    {
        private static readonly DateTime Hoy = new(2026, 11, 13);

        private readonly SqliteConnection _conexion;
        private readonly AppDbContext _context;
        private readonly PushFalso _push = new();
        private readonly SeguimientoService _servicio;

        private readonly Usuario _medico;
        private readonly Usuario _otroMedico;
        private readonly Usuario _administrador;

        public SeguimientoServiceTests()
        {
            _conexion = new SqliteConnection("DataSource=:memory:");
            _conexion.Open();

            _context = NuevoContexto();
            _context.Database.EnsureCreated();

            _medico = AgregarUsuario("medico", RolUsuario.MEDICO);
            _otroMedico = AgregarUsuario("otro", RolUsuario.MEDICO);
            _administrador = AgregarUsuario("admin", RolUsuario.ADMINISTRADOR);

            var reloj = new RelojFijo(new DateTimeOffset(Hoy.AddHours(9), TimeSpan.FromHours(-5)));
            var opciones = Options.Create(new SeguimientoOptions { DiasAnticipacion = 3 });

            _servicio = new SeguimientoService(
                _context,
                _push,
                reloj,
                opciones,
                NullLogger<SeguimientoService>.Instance);
        }

        public void Dispose()
        {
            _context.Dispose();
            _conexion.Dispose();
        }

        // ---------- Crear ----------

        [Fact]
        public async Task Crear_ConDatosValidos_QuedaPendiente()
        {
            var resultado = await _servicio.CrearAsync(10, _medico.IdUsuario, Hoy.AddDays(7), "  Control de presión  ");

            Assert.True(resultado.Exito);
            var guardada = await NuevoContexto().SeguimientoAlertas.SingleAsync();
            Assert.Equal(EstadoAlerta.Pendiente, guardada.Estado);
            Assert.Equal("Control de presión", guardada.Motivo);
            Assert.Equal(_medico.IdUsuario, guardada.IdMedicoResponsable);
        }

        [Fact]
        public async Task Crear_ParaHoy_SePermite()
        {
            var resultado = await _servicio.CrearAsync(10, _medico.IdUsuario, Hoy, "Curación");

            Assert.True(resultado.Exito);
        }

        [Fact]
        public async Task Crear_ConFechaPasada_SeRechaza()
        {
            var resultado = await _servicio.CrearAsync(10, _medico.IdUsuario, Hoy.AddDays(-1), "Control");

            Assert.False(resultado.Exito);
            Assert.Contains("pasado", resultado.Error);
            Assert.Empty(NuevoContexto().SeguimientoAlertas);
        }

        [Fact]
        public async Task Crear_ConMedicoInexistente_SeRechaza()
        {
            var resultado = await _servicio.CrearAsync(10, 999, Hoy.AddDays(1), "Control");

            Assert.False(resultado.Exito);
        }

        [Fact]
        public async Task Crear_ConMedicoInactivo_SeRechaza()
        {
            var inactivo = AgregarUsuario("inactivo", RolUsuario.MEDICO, activo: false);

            var resultado = await _servicio.CrearAsync(10, inactivo.IdUsuario, Hoy.AddDays(1), "Control");

            Assert.False(resultado.Exito);
        }

        [Fact]
        public async Task Crear_ConAdministradorComoResponsable_SeRechaza()
        {
            var resultado = await _servicio.CrearAsync(10, _administrador.IdUsuario, Hoy.AddDays(1), "Control");

            Assert.False(resultado.Exito);
        }

        [Fact]
        public async Task Crear_ConEnfermeroComoResponsable_SePermite()
        {
            var enfermero = AgregarUsuario("enfermero", RolUsuario.ENFERMERO);

            var resultado = await _servicio.CrearAsync(10, enfermero.IdUsuario, Hoy.AddDays(1), "Control");

            Assert.True(resultado.Exito);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public async Task Crear_SinMotivo_SeRechaza(string motivo)
        {
            var resultado = await _servicio.CrearAsync(10, _medico.IdUsuario, Hoy.AddDays(1), motivo);

            Assert.False(resultado.Exito);
        }

        // ---------- Pendientes ----------

        [Fact]
        public async Task ObtenerPendientes_NoIncluyeAlertasDeOtroMedico()
        {
            AgregarAlerta(_medico, Hoy.AddDays(1));
            AgregarAlerta(_otroMedico, Hoy.AddDays(1));

            var pendientes = await _servicio.ObtenerPendientesAsync(_medico.IdUsuario);

            Assert.Single(pendientes);
            Assert.All(pendientes, a => Assert.Equal(_medico.IdUsuario, a.IdMedicoResponsable));
        }

        [Fact]
        public async Task ObtenerPendientes_IncluyeVencidasYProximas_OrdenadasPorFecha()
        {
            var vencida = AgregarAlerta(_medico, Hoy.AddDays(-2));
            var deHoy = AgregarAlerta(_medico, Hoy);
            var ultimoDiaDeVentana = AgregarAlerta(_medico, Hoy.AddDays(3).AddHours(17));
            AgregarAlerta(_medico, Hoy.AddDays(4));
            AgregarAlerta(_medico, Hoy.AddDays(1), EstadoAlerta.Atendida);
            AgregarAlerta(_medico, Hoy.AddDays(1), EstadoAlerta.Descartada);

            var pendientes = await _servicio.ObtenerPendientesAsync(_medico.IdUsuario);

            Assert.Equal(
                new[] { vencida.IdAlerta, deHoy.IdAlerta, ultimoDiaDeVentana.IdAlerta },
                pendientes.Select(a => a.IdAlerta));
        }

        // ---------- Resolver ----------

        [Theory]
        [InlineData(EstadoAlerta.Atendida)]
        [InlineData(EstadoAlerta.Descartada)]
        public async Task Resolver_PorMedicoResponsable_CambiaEstado(EstadoAlerta nuevoEstado)
        {
            var alerta = AgregarAlerta(_medico, Hoy.AddDays(1));

            var resultado = await _servicio.ResolverAsync(alerta.IdAlerta, _medico.IdUsuario, nuevoEstado, " Se llamó al paciente ");

            Assert.Equal(ResultadoResolucion.Resuelta, resultado);
            var guardada = await NuevoContexto().SeguimientoAlertas.SingleAsync();
            Assert.Equal(nuevoEstado, guardada.Estado);
            Assert.Equal("Se llamó al paciente", guardada.ObservacionResolucion);
            Assert.NotNull(guardada.FechaResolucion);
        }

        [Fact]
        public async Task Resolver_PorOtroMedico_NoAutorizadoYNoCambia()
        {
            var alerta = AgregarAlerta(_medico, Hoy.AddDays(1));

            var resultado = await _servicio.ResolverAsync(alerta.IdAlerta, _otroMedico.IdUsuario, EstadoAlerta.Atendida, null);

            Assert.Equal(ResultadoResolucion.NoAutorizado, resultado);
            Assert.Equal(EstadoAlerta.Pendiente, (await NuevoContexto().SeguimientoAlertas.SingleAsync()).Estado);
        }

        [Fact]
        public async Task Resolver_AlertaYaResuelta_DevuelveYaResuelta()
        {
            var alerta = AgregarAlerta(_medico, Hoy.AddDays(1), EstadoAlerta.Descartada);

            var resultado = await _servicio.ResolverAsync(alerta.IdAlerta, _medico.IdUsuario, EstadoAlerta.Atendida, null);

            Assert.Equal(ResultadoResolucion.YaResuelta, resultado);
            Assert.Equal(EstadoAlerta.Descartada, (await NuevoContexto().SeguimientoAlertas.SingleAsync()).Estado);
        }

        [Fact]
        public async Task Resolver_AlertaInexistente_NoEncontrada()
        {
            var resultado = await _servicio.ResolverAsync(999, _medico.IdUsuario, EstadoAlerta.Atendida, null);

            Assert.Equal(ResultadoResolucion.NoEncontrada, resultado);
        }

        [Fact]
        public async Task Resolver_AVolverAPendiente_LanzaExcepcion()
        {
            var alerta = AgregarAlerta(_medico, Hoy.AddDays(1));

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
                () => _servicio.ResolverAsync(alerta.IdAlerta, _medico.IdUsuario, EstadoAlerta.Pendiente, null));
        }

        // ---------- Recordatorios push ----------

        [Fact]
        public async Task EnviarRecordatorios_NotificaSoloAlMedicoResponsableYUnaSolaVez()
        {
            var proxima = AgregarAlerta(_medico, Hoy.AddDays(2));
            AgregarAlerta(_medico, Hoy.AddDays(10));
            AgregarAlerta(_medico, Hoy.AddDays(1), EstadoAlerta.Atendida);

            var primeraPasada = await _servicio.EnviarRecordatoriosAsync();
            var segundaPasada = await _servicio.EnviarRecordatoriosAsync();

            Assert.Equal(1, primeraPasada);
            Assert.Equal(0, segundaPasada);
            var enviada = Assert.Single(_push.Enviadas);
            Assert.Equal(proxima.IdAlerta, enviada.IdAlerta);
            Assert.Equal(_medico.IdUsuario, enviada.IdMedicoResponsable);
        }

        [Fact]
        public async Task EnviarRecordatorios_SiElPushFalla_SeReintentaDespues()
        {
            AgregarAlerta(_medico, Hoy.AddDays(1));
            _push.Responder = false;

            var fallida = await _servicio.EnviarRecordatoriosAsync();
            _push.Responder = true;
            var reintento = await _servicio.EnviarRecordatoriosAsync();

            Assert.Equal(0, fallida);
            Assert.Equal(1, reintento);
            Assert.NotNull((await NuevoContexto().SeguimientoAlertas.SingleAsync()).FechaNotificacionPush);
        }

        [Fact]
        public async Task EnviarRecordatorios_SiElPushLanzaError_ContinuaConLasDemas()
        {
            var conError = AgregarAlerta(_medico, Hoy);
            AgregarAlerta(_medico, Hoy.AddDays(1));
            _push.IdAlertaQueFalla = conError.IdAlerta;

            var enviadas = await _servicio.EnviarRecordatoriosAsync();

            Assert.Equal(1, enviadas);
        }

        // ---------- Ayudas ----------

        private AppDbContext NuevoContexto()
        {
            var opciones = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(_conexion)
                .Options;
            return new AppDbContext(opciones);
        }

        private Usuario AgregarUsuario(string username, RolUsuario rol, bool activo = true)
        {
            using var context = NuevoContexto();
            var usuario = new Usuario
            {
                Nombre = username,
                Apellido = "Prueba",
                Username = username,
                Email = $"{username}@prueba.test",
                PasswordHash = "no-usado",
                Rol = rol,
                Activo = activo
            };
            context.Usuarios.Add(usuario);
            context.SaveChanges();
            return usuario;
        }

        private SeguimientoAlerta AgregarAlerta(Usuario medico, DateTime fechaControl, EstadoAlerta estado = EstadoAlerta.Pendiente)
        {
            using var context = NuevoContexto();
            var alerta = new SeguimientoAlerta
            {
                IdColaborador = 10,
                IdMedicoResponsable = medico.IdUsuario,
                FechaControl = fechaControl,
                Motivo = "Control",
                Estado = estado,
                FechaCreacion = Hoy.AddDays(-5)
            };
            context.SeguimientoAlertas.Add(alerta);
            context.SaveChanges();
            return alerta;
        }

        private sealed class RelojFijo : TimeProvider
        {
            private readonly DateTimeOffset _ahora;
            private readonly TimeZoneInfo _zona;

            public RelojFijo(DateTimeOffset ahora)
            {
                _ahora = ahora;
                _zona = TimeZoneInfo.CreateCustomTimeZone("Prueba", ahora.Offset, "Prueba", "Prueba");
            }

            public override DateTimeOffset GetUtcNow() => _ahora.ToUniversalTime();

            public override TimeZoneInfo LocalTimeZone => _zona;
        }

        private sealed class PushFalso : INotificacionPushService
        {
            public List<SeguimientoAlerta> Enviadas { get; } = [];

            public bool Responder { get; set; } = true;

            public int? IdAlertaQueFalla { get; set; }

            public Task<bool> EnviarRecordatorioAsync(SeguimientoAlerta alerta, CancellationToken cancellationToken = default)
            {
                if (alerta.IdAlerta == IdAlertaQueFalla)
                {
                    throw new InvalidOperationException("Fallo simulado");
                }

                if (Responder)
                {
                    Enviadas.Add(alerta);
                }

                return Task.FromResult(Responder);
            }
        }
    }
}
