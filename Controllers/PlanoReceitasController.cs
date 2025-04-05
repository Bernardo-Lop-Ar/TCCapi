using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HealthifyAPI.Models;
using HealthifyAPI.Data;

namespace HealthifyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlanoReceitasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PlanoReceitasController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlanoReceita>>> GetPlanoReceitas()
        {
            return await _context.PlanoReceitas.ToListAsync();
        }

        [HttpGet("{planoId}/{receitaId}")]
        public async Task<ActionResult<PlanoReceita>> GetPlanoReceita(int planoId, int receitaId)
        {
            var planoReceita = await _context.PlanoReceitas.FindAsync(planoId, receitaId);
            if (planoReceita == null) return NotFound();
            return planoReceita;
        }

        [HttpPost]
        public async Task<ActionResult<PlanoReceita>> PostPlanoReceita(PlanoReceita planoReceita)
        {
            _context.PlanoReceitas.Add(planoReceita);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetPlanoReceita), new { planoId = planoReceita.PlanoId, receitaId = planoReceita.ReceitaId }, planoReceita);
        }

        [HttpPut("{planoId}/{receitaId}")]
        public async Task<IActionResult> PutPlanoReceita(int planoId, int receitaId, PlanoReceita planoReceita)
        {
            if (planoId != planoReceita.PlanoId || receitaId != planoReceita.ReceitaId)
                return BadRequest();

            _context.Entry(planoReceita).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{planoId}/{receitaId}")]
        public async Task<IActionResult> DeletePlanoReceita(int planoId, int receitaId)
        {
            var planoReceita = await _context.PlanoReceitas.FindAsync(planoId, receitaId);
            if (planoReceita == null) return NotFound();

            _context.PlanoReceitas.Remove(planoReceita);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
