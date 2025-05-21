using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HealthifyAPI.Models;
using HealthifyAPI.Data;

namespace HealthifyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReceitasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReceitasController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Receita>>> GetReceitas()
        {
            return await _context.Receitas.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Receita>> GetReceita(int id)
        {
            var receita = await _context.Receitas.FindAsync(id);
            if (receita == null) return NotFound();
            return receita;
        }

        [HttpPost]
        public async Task<ActionResult<Receita>> PostReceita(Receita receita)
        {
            _context.Receitas.Add(receita);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetReceita), new { id = receita.ReceitaId }, receita);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutReceita(int id, Receita receita)
        {
            if (id != receita.ReceitaId)
                return BadRequest();

            var receitaExistente = await _context.Receitas.FindAsync(id);
            if (receitaExistente == null)
                return NotFound();

            receitaExistente.Nome = receita.Nome;
            receitaExistente.Ingredientes = receita.Ingredientes;
            receitaExistente.Instrucoes = receita.Instrucoes;
            receitaExistente.CaloriasPorPorcao = receita.CaloriasPorPorcao;
            receitaExistente.Categoria = receita.Categoria;
            receitaExistente.Tipo = receita.Tipo;

            await _context.SaveChangesAsync();

            return NoContent();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReceita(int id)
        {
            var receita = await _context.Receitas.FindAsync(id);
            if (receita == null) return NotFound();
            _context.Receitas.Remove(receita);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
