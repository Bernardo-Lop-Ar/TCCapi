using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HealthifyAPI.Models;
using HealthifyAPI.Data;

namespace HealthifyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NutricionistasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public NutricionistasController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Nutricionista>>> GetNutricionistas()
        {
            return await _context.Nutricionistas.Include(n => n.Usuario).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Nutricionista>> GetNutricionista(int id)
        {
            var nutricionista = await _context.Nutricionistas.Include(n => n.Usuario).FirstOrDefaultAsync(n => n.NutricionistaId == id);
            if (nutricionista == null) return NotFound();
            return nutricionista;
        }

        [HttpPost]
        public async Task<ActionResult<Nutricionista>> PostNutricionista(Nutricionista nutricionista)
        {
            _context.Nutricionistas.Add(nutricionista);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetNutricionista), new { id = nutricionista.NutricionistaId }, nutricionista);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutNutricionista(int id, Nutricionista nutricionista)
        {
            if (id != nutricionista.NutricionistaId) return BadRequest();
            _context.Entry(nutricionista).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNutricionista(int id)
        {
            var nutricionista = await _context.Nutricionistas.FindAsync(id);
            if (nutricionista == null) return NotFound();
            _context.Nutricionistas.Remove(nutricionista);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
