using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HealthifyAPI.Data;
using HealthifyAPI.Models;
using HealthifyAPI.Models.DTOs;

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

        private ConsultaDTO MapToDTO(Consulta consulta) => new ConsultaDTO
        {
            ConsultaId = consulta.ConsultaId,
            ClienteId = consulta.ClienteId,
            NutricionistaId = consulta.NutricionistaId,
            DataConsulta = consulta.DataConsulta,
            TipoConsulta = consulta.TipoConsulta,
            HoraConsulta = consulta.HoraConsulta,
            Status = consulta.Status,
            Observacoes = consulta.Observacoes,
            Cliente = new ClienteDTO
            {
                ClienteId = consulta.Cliente.ClienteId,
                UsuarioId = consulta.Cliente.UsuarioId,
                Nome = consulta.Cliente.Usuario?.Nome
            },
            Nutricionista = new NutricionistaDTO
            {
                NutricionistaId = consulta.Nutricionista.NutricionistaId,
                UsuarioId = consulta.Nutricionista.UsuarioId,
                Nome = consulta.Nutricionista.Usuario?.Nome
            }
        };

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ConsultaDTO>>> GetConsultas(int? clienteId, int? nutricionistaId, string? status)
        {
            var query = _context.Consultas
                .Include(c => c.Cliente).ThenInclude(c => c.Usuario)
                .Include(c => c.Nutricionista).ThenInclude(n => n.Usuario)
                .AsQueryable();

            if (clienteId.HasValue) query = query.Where(c => c.ClienteId == clienteId.Value);
            if (nutricionistaId.HasValue) query = query.Where(c => c.NutricionistaId == nutricionistaId.Value);
            if (!string.IsNullOrEmpty(status)) query = query.Where(c => c.Status == status);

            var consultas = await query.ToListAsync();
            return consultas.Select(MapToDTO).ToList();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ConsultaDTO>> GetConsulta(int id)
        {
            var consulta = await _context.Consultas
                .Include(c => c.Cliente).ThenInclude(c => c.Usuario)
                .Include(c => c.Nutricionista).ThenInclude(n => n.Usuario)
                .FirstOrDefaultAsync(c => c.ConsultaId == id);

            if (consulta == null) return NotFound();
            return MapToDTO(consulta);
        }

        [HttpPost]
        public async Task<ActionResult<ConsultaDTO>> PostConsulta(ConsultaDTO dto)
        {
            var cliente = await _context.Clientes.FindAsync(dto.ClienteId);
            var nutricionista = await _context.Nutricionistas.FindAsync(dto.NutricionistaId);

            if (cliente == null || nutricionista == null)
                return BadRequest("Cliente ou Nutricionista inválido.");

            var consulta = new Consulta
            {
                ClienteId = dto.ClienteId,
                NutricionistaId = dto.NutricionistaId,
                DataConsulta = dto.DataConsulta.Date,
                HoraConsulta = dto.HoraConsulta,
                TipoConsulta = dto.TipoConsulta,
                Status = dto.Status,
                Observacoes = dto.Observacoes
            };

            _context.Consultas.Add(consulta);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetConsulta), new { id = consulta.ConsultaId }, MapToDTO(consulta));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutConsulta(int id, ConsultaDTO dto)
        {
            if (id != dto.ConsultaId) return BadRequest();

            var consulta = await _context.Consultas.FindAsync(id);
            if (consulta == null) return NotFound();

            consulta.DataConsulta = dto.DataConsulta.Date.AddHours(12);
            consulta.TipoConsulta = dto.TipoConsulta;
            consulta.Status = dto.Status;
            consulta.Observacoes = dto.Observacoes;

            _context.Entry(consulta).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> AtualizarStatus(int id, [FromBody] string novoStatus)
        {
            var consulta = await _context.Consultas.FindAsync(id);
            if (consulta == null) return NotFound();

            consulta.Status = novoStatus;
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

        [HttpGet("dias-indisponiveis/{nutricionistaId}")]
        public async Task<ActionResult<IEnumerable<string>>> GetDiasIndisponiveis(int nutricionistaId)
        {
            var datas = await _context.Consultas
                .Where(c => c.NutricionistaId == nutricionistaId)
                .Select(c => c.DataConsulta.Date)
                .Distinct()
                .ToListAsync();

            var datasFormatadas = datas.Select(d => d.ToString("yyyy-MM-dd")).ToList();
            return datasFormatadas;
        }

        [HttpGet("horarios-ocupados")]
        public async Task<ActionResult<IEnumerable<HorarioConsultaDTO>>> GetHorariosOcupados([FromQuery] string data)
        {
            if (!DateTime.TryParse(data, out DateTime dataConsulta))
            {
                return BadRequest("Data inválida.");
            }

            var horarios = await _context.Consultas
                .Include(c => c.Nutricionista).ThenInclude(n => n.Usuario)
                .Where(c => c.DataConsulta.Date == dataConsulta.Date)
                .Select(c => new HorarioConsultaDTO
                {
                    Hora = c.HoraConsulta,
                    NutricionistaId = c.NutricionistaId,
                    NutricionistaNome = c.Nutricionista.Usuario.Nome
                })
                .ToListAsync();

            return horarios;
        }
    }
}
