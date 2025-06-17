using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HealthifyAPI.Models;
using HealthifyAPI.Data;
using HealthifyAPI.Models.DTOs;

namespace HealthifyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ClientesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClienteResumoDTO>>> GetClientes()
        {
            var clientes = await _context.Clientes
                .Include(c => c.Usuario)
                .Select(c => new ClienteResumoDTO
                {
                    ClienteId = c.ClienteId,
                    Peso = c.Peso,
                    Altura = c.Altura,
                    Objetivo = c.Objetivo,
                    NivelAtividade = c.NivelAtividade,
                    PreferenciasAlimentares = c.PreferenciasAlimentares,
                    DoencasPreexistentes = c.DoencasPreexistentes,
                    UsuarioId = c.UsuarioId,
                    Nome = c.Usuario.Nome,
                    Email = c.Usuario.Email,
                    TipoUsuario = c.Usuario.TipoUsuario,
                    cpf = c.Usuario.cpf,
                    telefone = c.Usuario.telefone,
                    DataNascimento = c.Usuario.DataNascimento,
                    Sexo = c.Usuario.Sexo,
                    Endereco = c.Usuario.Endereco
                })
                .ToListAsync();

            return clientes;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Cliente>> GetCliente(int id)
        {
            var cliente = await _context.Clientes.Include(c => c.Usuario).FirstOrDefaultAsync(c => c.ClienteId == id);
            if (cliente == null) return NotFound();
            return cliente;
        }

        [HttpPost]
        public async Task<ActionResult<Cliente>> PostCliente(Cliente cliente)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                _context.Clientes.Add(cliente);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetCliente), new { id = cliente.ClienteId }, cliente);
            }
            catch (DbUpdateException dbEx)
            {
                var innerMessage = dbEx.InnerException != null ? dbEx.InnerException.Message : dbEx.Message;
                Console.WriteLine($"Erro ao salvar Cliente: {innerMessage}");
                return StatusCode(500, $"Erro ao salvar Cliente: {innerMessage}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro inesperado: {ex.Message}");
                return StatusCode(500, $"Erro inesperado: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCliente(int id, Cliente cliente)
        {
            ModelState.Remove("Usuario");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != cliente.ClienteId) return BadRequest();

            _context.Entry(cliente).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClienteExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCliente(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null) return NotFound();
            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool ClienteExists(int id)
        {
            return _context.Clientes.Any(e => e.ClienteId == id);
        }
        [HttpPost("respostas")]
        public async Task<IActionResult> PostRespostas([FromBody] List<QuestionarioResposta> respostas)
        {
            if (respostas == null || respostas.Count == 0)
                return BadRequest("Nenhuma resposta enviada.");

            try
            {

                await _context.QuestionarioRespostas.AddRangeAsync(respostas);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Respostas salvas com sucesso." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao salvar respostas: {ex.Message}");
                return StatusCode(500, $"Erro ao salvar respostas: {ex.Message}");
            }
        }
    }
}
