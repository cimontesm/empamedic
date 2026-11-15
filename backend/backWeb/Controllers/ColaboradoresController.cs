using backWeb.Data;
using backWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ColaboradoresController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ColaboradoresController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Colaboradores
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Colaborador>>> GetColaboradores()
        {
            return await _context.Colaboradores.ToListAsync();
        }

        // GET: api/Colaboradores/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Colaborador>> GetColaborador(int id)
        {
            var colaborador = await _context.Colaboradores.FindAsync(id);

            if (colaborador == null)
            {
                return NotFound();
            }

            return colaborador;
        }

        // POST: api/Colaboradores
        [HttpPost]
        public async Task<ActionResult<Colaborador>> PostColaborador(Colaborador colaborador)
        {
            _context.Colaboradores.Add(colaborador);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetColaborador), new { id = colaborador.Id }, colaborador);
        }

        // PUT: api/Colaboradores/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutColaborador(int id, Colaborador colaborador)
        {
            if (id != colaborador.Id)
            {
                return BadRequest();
            }

            _context.Entry(colaborador).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ColaboradorExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/Colaboradores/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteColaborador(int id)
        {
            var colaborador = await _context.Colaboradores.FindAsync(id);
            if (colaborador == null)
            {
                return NotFound();
            }

            _context.Colaboradores.Remove(colaborador);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ColaboradorExists(int id)
        {
            return _context.Colaboradores.Any(e => e.Id == id);
        }
    }
}
