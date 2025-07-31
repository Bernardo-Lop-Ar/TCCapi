using HealthifyAPI.Data;
using HealthifyAPI.Models;
using HealthifyAPI.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace HealthifyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiarioController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DiarioController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Diario/cliente/{clienteId}
        // Busca todos os registos do diário para um cliente específico
        [HttpGet("cliente/{clienteId}")]
        public async Task<IActionResult> GetDiarioPorCliente(int clienteId)
        {
            var diarioEntradas = await _context.DiarioEntradas
                .Where(d => d.ClienteId == clienteId)
                .OrderByDescending(d => d.DataEntrada) // Ordena do mais recente para o mais antigo
                .ToListAsync();

            if (!diarioEntradas.Any())
            {
                return NotFound("Nenhum registo no diário encontrado para este cliente.");
            }

            return Ok(diarioEntradas);
        }

        // POST: api/Diario
        // Cria um novo registo no diário
        [HttpPost]
        public async Task<IActionResult> CriarEntradaDiario([FromBody] DiarioEntradaDto diarioDto)
        {
            if (diarioDto == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var novaEntrada = new DiarioEntrada
            {
                ClienteId = diarioDto.ClienteId,
                DataEntrada = diarioDto.DataEntrada.Date, // Garante que apenas a data seja guardada
                Refeicoes = diarioDto.Refeicoes,
                Sintomas = diarioDto.Sintomas
            };

            await _context.DiarioEntradas.AddAsync(novaEntrada);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDiarioPorCliente), new { clienteId = novaEntrada.ClienteId }, novaEntrada);
        }
    }
}
