using backWeb.Models;

namespace backWeb.Services
{
    /// <summary>
    /// Consultas sobre las plantillas de receta (RF06, HU4).
    /// </summary>
    public interface IPlantillaRecetaService
    {
        /// <summary>
        /// Lista todas las plantillas ordenadas por nombre, sin cargar sus ítems.
        /// </summary>
        Task<IReadOnlyList<PlantillaReceta>> ListarAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Obtiene una plantilla con sus ítems y el medicamento de cada ítem,
        /// o null si no existe.
        /// </summary>
        Task<PlantillaReceta?> ObtenerConItemsAsync(int idPlantilla, CancellationToken cancellationToken = default);
    }
}
