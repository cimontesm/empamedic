using backWeb.Models;
using backWeb.Services;

namespace backWeb.Tests
{
    public class MedicamentoServiceTests : IDisposable
    {
        private readonly SqliteEnMemoria _db = new();

        public MedicamentoServiceTests()
        {
            using var context = _db.CrearContexto();
            context.Medicamentos.AddRange(
                CrearMedicamento("Paracetamol", "Paracetamol"),
                CrearMedicamento("Ibuprofeno", "Ibuprofeno"),
                CrearMedicamento("Buscapina", "Butilescopolamina"),
                CrearMedicamento("Amoxicilina", "Amoxicilina"));
            context.SaveChanges();
        }

        public void Dispose()
        {
            _db.Dispose();
        }

        private static Medicamento CrearMedicamento(string nombreComercial, string principioActivo)
        {
            return new Medicamento
            {
                NombreComercial = nombreComercial,
                PrincipioActivo = principioActivo,
                Presentacion = "Tabletas",
                Concentracion = "500 mg",
            };
        }

        private async Task<IReadOnlyList<Medicamento>> BuscarAsync(string texto)
        {
            using var context = _db.CrearContexto();
            var servicio = new MedicamentoService(context);
            return await servicio.BuscarPorNombreAsync(texto);
        }

        [Fact]
        public async Task Buscar_ParteDelNombre_DevuelveLosQueLoContienen()
        {
            // "cetam" está en medio de "Paracetamol".
            var resultado = await BuscarAsync("cetam");

            Assert.Equal("Paracetamol", Assert.Single(resultado).NombreComercial);
        }

        [Fact]
        public async Task Buscar_PorPrincipioActivo_DevuelveElMedicamento()
        {
            // "Buscapina" no contiene "escopolamina"; su principio activo sí.
            var resultado = await BuscarAsync("escopolamina");

            Assert.Equal("Buscapina", Assert.Single(resultado).NombreComercial);
        }

        [Fact]
        public async Task Buscar_CoincideEnNombreYPrincipioActivo_NoRepiteElMedicamento()
        {
            // "Paracetamol" coincide en los dos campos, pero debe salir una sola vez.
            var resultado = await BuscarAsync("paracetamol");

            Assert.Equal("Paracetamol", Assert.Single(resultado).NombreComercial);
        }

        [Fact]
        public async Task Buscar_SinDistinguirMayusculas()
        {
            var resultado = await BuscarAsync("PARACE");

            Assert.Equal("Paracetamol", Assert.Single(resultado).NombreComercial);
        }

        [Fact]
        public async Task Buscar_VariosResultados_VienenOrdenadosPorNombre()
        {
            // "i" aparece en Amoxicilina, Buscapina e Ibuprofeno.
            var resultado = await BuscarAsync("i");

            Assert.Equal(
                new[] { "Amoxicilina", "Buscapina", "Ibuprofeno" },
                resultado.Select(m => m.NombreComercial));
        }

        [Fact]
        public async Task Buscar_TextoConEspacios_IgnoraLosEspacios()
        {
            var resultado = await BuscarAsync("  ibu  ");

            Assert.Equal("Ibuprofeno", Assert.Single(resultado).NombreComercial);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public async Task Buscar_TextoVacio_DevuelveListaVacia(string texto)
        {
            Assert.Empty(await BuscarAsync(texto));
        }

        [Fact]
        public async Task Buscar_SinCoincidencias_DevuelveListaVacia()
        {
            Assert.Empty(await BuscarAsync("Omeprazol"));
        }

        [Theory]
        [InlineData("%")]
        [InlineData("_")]
        public async Task Buscar_ComodinesDeLike_NoDevuelvenTodoElCatalogo(string texto)
        {
            Assert.Empty(await BuscarAsync(texto));
        }
    }
}
