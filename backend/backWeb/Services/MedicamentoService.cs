using backWeb.Data;
using backWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace backWeb.Services
{
    public class MedicamentoService : IMedicamentoService
    {
        private readonly AppDbContext _context;

        public MedicamentoService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<Medicamento>> BuscarPorNombreAsync(string texto, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(texto))
            {
                return Array.Empty<Medicamento>();
            }

            // LIKE '%texto%'. Se escapan %, _ y [ para que el usuario no pueda usarlos
            // como comodines (por ejemplo, buscar "%" no debe devolver todo el catálogo).
            var patron = $"%{EscaparLike(texto.Trim())}%";

            return await _context.Medicamentos
                .AsNoTracking()
                .Where(m => EF.Functions.Like(m.NombreComercial, patron, @"\")
                    || EF.Functions.Like(m.PrincipioActivo, patron, @"\"))
                .OrderBy(m => m.NombreComercial)
                .ToListAsync(cancellationToken);
        }

        private static string EscaparLike(string texto)
        {
            return texto
                .Replace(@"\", @"\\")
                .Replace("%", @"\%")
                .Replace("_", @"\_")
                .Replace("[", @"\[");
        }
    }
}
