using backWeb.Models;
using backWeb.Services;

namespace backWeb.Tests
{
    public class PlantillaRecetaServiceTests : IDisposable
    {
        private readonly SqliteEnMemoria _db = new();
        private readonly int _idPlantillaGripe;
        private readonly int _idPlantillaVacia;

        public PlantillaRecetaServiceTests()
        {
            using var context = _db.CrearContexto();

            var paracetamol = new Medicamento
            {
                NombreComercial = "Paracetamol",
                PrincipioActivo = "Paracetamol",
                Presentacion = "Tabletas",
                Concentracion = "500 mg",
            };
            var loratadina = new Medicamento
            {
                NombreComercial = "Loratadina",
                PrincipioActivo = "Loratadina",
                Presentacion = "Tabletas",
                Concentracion = "10 mg",
            };

            var gripe = new PlantillaReceta
            {
                NombrePlantilla = "Gripe común",
                DiagnosticoAsociado = "J00 Rinofaringitis aguda",
                Descripcion = "Tratamiento sintomático",
            };
            gripe.AgregarItem(paracetamol, "1 tableta", "cada 8 horas", "3 días", "Tomar después de las comidas");
            gripe.AgregarItem(loratadina, "1 tableta", "cada 24 horas", "5 días", string.Empty);

            var vacia = new PlantillaReceta
            {
                NombrePlantilla = "Cefalea",
                DiagnosticoAsociado = "R51 Cefalea",
                Descripcion = "Sin ítems todavía",
            };

            context.PlantillasReceta.AddRange(gripe, vacia);
            context.SaveChanges();

            _idPlantillaGripe = gripe.IdPlantilla;
            _idPlantillaVacia = vacia.IdPlantilla;
        }

        public void Dispose()
        {
            _db.Dispose();
        }

        [Fact]
        public async Task Listar_DevuelveTodasOrdenadasPorNombre()
        {
            using var context = _db.CrearContexto();
            var servicio = new PlantillaRecetaService(context);

            var plantillas = await servicio.ListarAsync();

            Assert.Equal(new[] { "Cefalea", "Gripe común" }, plantillas.Select(p => p.NombrePlantilla));
        }

        [Fact]
        public async Task Listar_NoCargaLosItems()
        {
            using var context = _db.CrearContexto();
            var servicio = new PlantillaRecetaService(context);

            var plantillas = await servicio.ListarAsync();

            Assert.All(plantillas, p => Assert.Empty(p.Items));
        }

        [Fact]
        public async Task Listar_SinPlantillas_DevuelveListaVacia()
        {
            using var dbVacia = new SqliteEnMemoria();
            using var context = dbVacia.CrearContexto();
            var servicio = new PlantillaRecetaService(context);

            Assert.Empty(await servicio.ListarAsync());
        }

        [Fact]
        public async Task ObtenerConItems_DevuelveLosItemsConSuMedicamento()
        {
            using var context = _db.CrearContexto();
            var servicio = new PlantillaRecetaService(context);

            var plantilla = await servicio.ObtenerConItemsAsync(_idPlantillaGripe);

            Assert.NotNull(plantilla);
            Assert.Equal("Gripe común", plantilla.NombrePlantilla);
            Assert.Equal(2, plantilla.Items.Count);

            var itemParacetamol = Assert.Single(plantilla.Items, i => i.Medicamento.NombreComercial == "Paracetamol");
            Assert.Equal("1 tableta", itemParacetamol.DosisSugerida);
            Assert.Equal("cada 8 horas", itemParacetamol.FrecuenciaSugerida);
            Assert.Equal("3 días", itemParacetamol.DuracionSugerida);
            Assert.Equal("Tomar después de las comidas", itemParacetamol.IndicacionesSugeridas);
        }

        [Fact]
        public async Task ObtenerConItems_PlantillaSinItems_DevuelveListaVacia()
        {
            using var context = _db.CrearContexto();
            var servicio = new PlantillaRecetaService(context);

            var plantilla = await servicio.ObtenerConItemsAsync(_idPlantillaVacia);

            Assert.NotNull(plantilla);
            Assert.Empty(plantilla.Items);
        }

        [Fact]
        public async Task ObtenerConItems_IdInexistente_DevuelveNull()
        {
            using var context = _db.CrearContexto();
            var servicio = new PlantillaRecetaService(context);

            Assert.Null(await servicio.ObtenerConItemsAsync(9999));
        }
    }
}
