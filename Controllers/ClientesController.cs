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
            ModelState.Remove("Usuario");

            foreach (var key in ModelState.Keys.Where(k => k.StartsWith("Usuario")).ToList())
            {
                ModelState.Remove(key);
            }
            
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCliente), new { id = cliente.ClienteId }, cliente);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCliente(int id, Cliente cliente)
        {
            ModelState.Remove("Usuario");  

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != cliente.ClienteId) return BadRequest();
            _context.Entry(cliente).State = EntityState.Modified;
            await _context.SaveChangesAsync();
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
    }
}
