using backWeb.Models;

namespace backWeb.Services
{
    /// <summary>
    /// Consultas sobre el catálogo de medicamentos (buscador de fármacos en la consulta, HU1).
    /// </summary>
    public interface IMedicamentoService
    {
        /// <summary>
        /// Busca medicamentos cuyo nombre comercial o principio activo contenga el texto,
        /// sin distinguir mayúsculas. Si el texto está vacío devuelve una lista vacía.
        /// </summary>
        Task<IReadOnlyList<Medicamento>> BuscarPorNombreAsync(string texto, CancellationToken cancellationToken = default);
    }
}
