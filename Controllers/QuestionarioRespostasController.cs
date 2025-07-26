

using HealthifyAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace HealthifyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionarioRespostasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public QuestionarioRespostasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/QuestionarioRespostas/cliente/{clienteId}
        [HttpGet("cliente/{clienteId}")]
        public async Task<IActionResult> GetRespostasPorCliente(int clienteId)
        {
            var respostas = await _context.QuestionarioRespostas
                .Include(r => r.Pergunta) // Importante: Inclui os dados da pergunta relacionada
                .Where(r => r.ClienteId == clienteId)
                .ToListAsync();

            // Se a lista estiver vazia, retorna 404, que o seu app já sabe tratar.
            if (!respostas.Any())
            {
                return NotFound("Nenhuma resposta encontrada para este cliente.");
            }

            // Selecionamos os dados para evitar loops infinitos e enviar só o necessário
            var resultado = respostas.Select(r => new
            {
                r.QuestionarioRespostaId,
                r.ClienteId,
                r.PerguntaId,
                r.RespostaTexto,
                r.DataResposta,
                Pergunta = new { r.Pergunta.Texto } // Envia o texto da pergunta junto
            });

            return Ok(resultado);
        }
    }
}