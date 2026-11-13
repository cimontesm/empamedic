using backWeb.Services;

// Va en el namespace de DependencyInjection (convención de .NET) para que Program.cs
// pueda llamar a AddServiciosConsultaMedica() sin agregar otro "using".
namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Registra los servicios del punto 3 (consulta médica, recetas y plantillas).
    /// Están en un archivo aparte para que Program.cs solo tenga una línea de este punto
    /// y no choque con los cambios de otras ramas.
    /// </summary>
    public static class ServiciosConsultaMedicaExtensions
    {
        public static IServiceCollection AddServiciosConsultaMedica(this IServiceCollection services)
        {
            // Scoped: una instancia por petición HTTP, igual que el AppDbContext que usan.
            services.AddScoped<IMedicamentoService, MedicamentoService>();
            services.AddScoped<IPlantillaRecetaService, PlantillaRecetaService>();

            return services;
        }
    }
}
