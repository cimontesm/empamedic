using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace backWeb.Tests
{
    /// <summary>
    /// Pruebas de humo: la API arranca y responde.
    /// No necesitan SQL Server porque no consultan la base de datos.
    /// </summary>
    public class ApiArranqueTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public ApiArranqueTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Health_RespondeOk()
        {
            using var client = _factory.CreateClient();

            var respuesta = await client.GetAsync("/health");

            Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);
        }
    }
}
