using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HealthifyAPI.Models;
using HealthifyAPI.Data;

namespace HealthifyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProgressoClienteController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProgressoClienteController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProgressoCliente>>> GetProgresso()
        {
            return await _context.ProgressoCliente.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProgressoCliente>> GetProgresso(int id)
        {
            var progresso = await _context.ProgressoCliente.FindAsync(id);
            if (progresso == null) return NotFound();
            return progresso;
        }

        [HttpPost]
        public async Task<ActionResult<ProgressoCliente>> PostProgresso(ProgressoCliente progresso)
        {
            _context.ProgressoCliente.Add(progresso);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetProgresso), new { id = progresso.ProgressoId }, progresso);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutProgresso(int id, ProgressoCliente progresso)
        {
            if (id != progresso.ProgressoId) return BadRequest();

            _context.Entry(progresso).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProgresso(int id)
        {
            var progresso = await _context.ProgressoCliente.FindAsync(id);
            if (progresso == null) return NotFound();

            _context.ProgressoCliente.Remove(progresso);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
