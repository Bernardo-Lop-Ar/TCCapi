using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HealthifyAPI.Models;
using HealthifyAPI.Data;

namespace HealthifyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConsultasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ConsultasController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
public async Task<ActionResult<IEnumerable<Consulta>>> GetConsultas(int? clienteId, int? nutricionistaId, string status)
{
    var consultas = _context.Consultas.AsQueryable();

    if (clienteId.HasValue)
        consultas = consultas.Where(c => c.ClienteId == clienteId.Value);

    if (nutricionistaId.HasValue)
        consultas = consultas.Where(c => c.NutricionistaId == nutricionistaId.Value);

    if (!string.IsNullOrEmpty(status))
        consultas = consultas.Where(c => c.Status == status);

    return await consultas.ToListAsync();
}


        [HttpGet("{id}")]
        public async Task<ActionResult<Consulta>> GetConsulta(int id)
        {
            var consulta = await _context.Consultas.FindAsync(id);
            if (consulta == null) return NotFound();
            return consulta;
        }

        [HttpPost]
public async Task<ActionResult<Consulta>> PostConsulta(Consulta consulta)
{
    // Verifica se o ClienteId e NutricionistaId são válidos
    var clienteExistente = await _context.Clientes.FindAsync(consulta.ClienteId);
    var nutricionistaExistente = await _context.Nutricionistas.FindAsync(consulta.NutricionistaId);

    if (clienteExistente == null || nutricionistaExistente == null)
    {
        return BadRequest("Cliente ou Nutricionista inválido.");
    }

    _context.Consultas.Add(consulta);
    await _context.SaveChangesAsync();
    return CreatedAtAction(nameof(GetConsulta), new { id = consulta.ConsultaId }, consulta);
}


        [HttpPut("{id}")]
        public async Task<IActionResult> PutConsulta(int id, Consulta consulta)
        {
            if (id != consulta.ConsultaId) return BadRequest();
            _context.Entry(consulta).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteConsulta(int id)
        {
            var consulta = await _context.Consultas.FindAsync(id);
            if (consulta == null) return NotFound();
            _context.Consultas.Remove(consulta);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        [HttpPatch("{id}/status")]
public async Task<IActionResult> AtualizarStatus(int id, [FromBody] string novoStatus)
{
    var consulta = await _context.Consultas.FindAsync(id);
    if (consulta == null)
    {
        return NotFound();
    }

    consulta.Status = novoStatus;
    _context.Entry(consulta).State = EntityState.Modified;
    await _context.SaveChangesAsync();
    
    return NoContent();
}

    }
}
