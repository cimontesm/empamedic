using backWeb.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace backWeb.Tests
{
    /// <summary>
    /// Base de datos SQLite en memoria para probar servicios sin SQL Server (el CI no lo tiene).
    /// La base vive mientras la conexión esté abierta, así que cada prueba que crea un
    /// SqliteEnMemoria arranca con una base nueva y vacía.
    /// </summary>
    public sealed class SqliteEnMemoria : IDisposable
    {
        private readonly SqliteConnection _conexion;
        private readonly DbContextOptions<AppDbContext> _opciones;

        public SqliteEnMemoria()
        {
            _conexion = new SqliteConnection("DataSource=:memory:");
            _conexion.Open();

            _opciones = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(_conexion)
                .Options;

            // Crea las tablas a partir del modelo (no usa las migraciones de SQL Server).
            using var context = CrearContexto();
            context.Database.EnsureCreated();
        }

        /// <summary>
        /// Crea un contexto nuevo sobre la misma base. Usar un contexto para preparar
        /// los datos y otro para el servicio evita que la prueba lea datos que solo
        /// están en la memoria del primer contexto.
        /// </summary>
        public AppDbContext CrearContexto()
        {
            return new AppDbContext(_opciones);
        }

        public void Dispose()
        {
            _conexion.Dispose();
        }
    }
}
