using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HealthifyAPI.Models;
using HealthifyAPI.Data;
using HealthifyAPI.Models.DTOs;
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

        [HttpGet("{id}")]
        public async Task<ActionResult<NutricionistaDTO>> GetNutricionista(int id)
        {
            var nutricionista = await _context.Nutricionistas
                .Include(n => n.Usuario)
                .FirstOrDefaultAsync(n => n.NutricionistaId == id);

            if (nutricionista == null)
                return NotFound();

            var dto = new NutricionistaDTO
            {
                NutricionistaId = nutricionista.NutricionistaId,
                UsuarioId = nutricionista.UsuarioId,
                Nome = nutricionista.Usuario?.Nome,
                Email = nutricionista.Usuario?.Email,
                Telefone = nutricionista.Usuario?.telefone,
                Sexo = nutricionista.Usuario?.Sexo,
                Cpf = nutricionista.Usuario?.cpf,
                Endereco = nutricionista.Usuario?.Endereco,
                DataNascimento = nutricionista.Usuario?.DataNascimento,
                Especialidade = nutricionista.Especialidade,
                Descricao = nutricionista.Descricao,
                FotoPerfil = nutricionista.FotoPerfil
            };

            return dto;
        }


        [HttpPost]
        public async Task<ActionResult<Nutricionista>> PostNutricionista(Nutricionista nutricionista)
        {

            foreach (var key in ModelState.Keys.Where(k => k.StartsWith("Usuario")).ToList())
            {
                ModelState.Remove(key);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Nutricionistas.Add(nutricionista);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetNutricionista), new { id = nutricionista.NutricionistaId }, nutricionista);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutNutricionista(int id, Nutricionista nutricionista)
        {
            if (id != nutricionista.NutricionistaId) return BadRequest();


            foreach (var key in ModelState.Keys.Where(k => k.StartsWith("Usuario")).ToList())
            {
                ModelState.Remove(key);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

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

        [HttpGet("respostas/{clienteId}")]
        public async Task<ActionResult<IEnumerable<QuestionarioResposta>>> GetRespostasPorCliente(int clienteId)
        {
            var respostas = await _context.QuestionarioRespostas
                                          .Where(q => q.ClienteId == clienteId)
                                          .ToListAsync();

            if (respostas == null || respostas.Count == 0)
                return NotFound($"Nenhuma resposta encontrada para o cliente com id {clienteId}");

            return respostas;
        }
    }
}
