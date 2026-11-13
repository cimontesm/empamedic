using backWeb.Data;
using backWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace backWeb.Services
{
    public class PlantillaRecetaService : IPlantillaRecetaService
    {
        private readonly AppDbContext _context;

        public PlantillaRecetaService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<PlantillaReceta>> ListarAsync(CancellationToken cancellationToken = default)
        {
            return await _context.PlantillasReceta
                .AsNoTracking()
                .OrderBy(p => p.NombrePlantilla)
                .ToListAsync(cancellationToken);
        }

        public async Task<PlantillaReceta?> ObtenerConItemsAsync(int idPlantilla, CancellationToken cancellationToken = default)
        {
            return await _context.PlantillasReceta
                .AsNoTracking()
                .Include(p => p.Items)
                    .ThenInclude(i => i.Medicamento)
                .FirstOrDefaultAsync(p => p.IdPlantilla == idPlantilla, cancellationToken);
        }
    }
}
