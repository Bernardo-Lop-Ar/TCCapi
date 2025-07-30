using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HealthifyAPI.Models;
using HealthifyAPI.Data;
using HealthifyAPI.Models.DTOs;
using System.Linq; // Necessário para o .Select()

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
            // Este método está correto para buscar a lista inicial de planos.
            return await _context.PlanosAlimentares.ToListAsync();
        }

        // --- MÉTODO GET POR ID ATUALIZADO ---
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetPlanoComDetalhes(int id)
        {
            var plano = await _context.PlanosAlimentares
                .Include(p => p.PlanoReceita)
                    .ThenInclude(pr => pr.Receita) // Garante que os detalhes da receita sejam carregados
                .FirstOrDefaultAsync(p => p.PlanoId == id);

            if (plano == null)
            {
                return NotFound();
            }

            // Criamos um objeto de resposta personalizado para garantir que todos os dados
            // sejam enviados corretamente, sem problemas de referência circular.
            var resultado = new
            {
                plano.PlanoId,
                plano.ClienteId,
                plano.NutricionistaId,
                plano.NomePlano,
                plano.DataInicio,
                plano.DataFim,
                plano.Observacoes,
                // Mapeamos a lista de receitas para a estrutura que o frontend espera
                PlanoReceita = plano.PlanoReceita.Select(pr => new {
                    pr.PlanoId,
                    pr.ReceitaId,
                    pr.QuantidadePorcao,
                    pr.DiaSemana,
                    pr.Refeicao,
                    // Incluímos o objeto da receita completo
                    Receita = new {
                        pr.Receita.ReceitaId,
                        pr.Receita.Nome,
                        pr.Receita.Instrucoes,
                        pr.Receita.Ingredientes,
                        pr.Receita.CaloriasPorPorcao,
                    }
                }).ToList()
            };

            return Ok(resultado);
        }


        [HttpPost]
        public async Task<ActionResult<PlanoAlimentar>> PostPlano(PlanoAlimentar plano)
        {
            _context.PlanosAlimentares.Add(plano);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetPlanoComDetalhes), new { id = plano.PlanoId }, plano);
        }

        [HttpPost("com-receitas")]
        public async Task<IActionResult> CriarPlanoComReceitas([FromBody] PlanoAlimentarComReceitasDto planoDto)
        {
            if (planoDto == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var novoPlano = new PlanoAlimentar
                    {
                        ClienteId = planoDto.ClienteId,
                        NutricionistaId = planoDto.NutricionistaId,
                        NomePlano = planoDto.NomePlano,
                        DataInicio = planoDto.DataInicio,
                        DataFim = planoDto.DataFim,
                        Observacoes = planoDto.Observacoes
                    };

                    _context.PlanosAlimentares.Add(novoPlano);
                    await _context.SaveChangesAsync();

                    if (planoDto.Receitas != null && planoDto.Receitas.Any())
                    {
                        var planoReceitas = planoDto.Receitas.Select(r => new PlanoReceita
                        {
                            PlanoId = novoPlano.PlanoId,
                            ReceitaId = r.ReceitaId,
                            QuantidadePorcao = r.QuantidadePorcao,
                            DiaSemana = r.DiaSemana,
                            Refeicao = r.Refeicao
                        }).ToList();

                        await _context.PlanoReceitas.AddRangeAsync(planoReceitas);
                        await _context.SaveChangesAsync();
                    }

                    await transaction.CommitAsync();
                    return Ok(new { message = "Plano alimentar e receitas associadas foram salvos com sucesso.", planoId = novoPlano.PlanoId });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine($"--- ERRO AO SALVAR PLANO COM RECEITAS ---");
                    Console.WriteLine(ex.InnerException?.Message ?? ex.Message);
                    Console.WriteLine($"-----------------------------------------");
                    return StatusCode(500, $"Erro interno ao salvar o plano: {ex.Message}");
                }
            }
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
