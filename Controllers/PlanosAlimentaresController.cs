using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HealthifyAPI.Models;
using HealthifyAPI.Data;
using HealthifyAPI.Models.DTOs;

namespace HealthifyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlanosAlimentaresController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PlanosAlimentaresController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlanoAlimentar>>> GetPlanos()
        {
            return await _context.PlanosAlimentares.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PlanoAlimentar>> GetPlano(int id)
        {
            var plano = await _context.PlanosAlimentares
                .Include(p => p.PlanoReceita)
                .ThenInclude(pr => pr.Receita)
                .FirstOrDefaultAsync(p => p.PlanoId == id);

            if (plano == null) return NotFound();
            return plano;
        }


        [HttpPost]
        public async Task<ActionResult<PlanoAlimentar>> PostPlano(PlanoAlimentar plano)
        {
            _context.PlanosAlimentares.Add(plano);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetPlano), new { id = plano.PlanoId }, plano);
        }

        [HttpPost("com-receitas")]
        public async Task<ActionResult<PlanoAlimentar>> PostPlanoComReceitas(CriarPlanoDTO dto)
        {
            var plano = new PlanoAlimentar
            {
                ClienteId = dto.ClienteId,
                NutricionistaId = dto.NutricionistaId,
                NomePlano = dto.NomePlano,
                DataInicio = dto.DataInicio,
                DataFim = dto.DataFim,
                Observacoes = dto.Observacoes,
                PlanoReceita = dto.Receitas.Select(r => new PlanoReceita
                {
                    ReceitaId = r.ReceitaId,
                    QuantidadePorcao = r.QuantidadePorcao,
                    DiaSemana = r.DiaSemana,
                    Refeicao = r.Refeicao
                }).ToList()
            };

            _context.PlanosAlimentares.Add(plano);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPlano), new { id = plano.PlanoId }, plano);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> PutPlano(int id, PlanoAlimentar plano)
        {
            if (id != plano.PlanoId) return BadRequest();
            _context.Entry(plano).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlano(int id)
        {
            var plano = await _context.PlanosAlimentares.FindAsync(id);
            if (plano == null) return NotFound();
            _context.PlanosAlimentares.Remove(plano);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
