using HealthifyAPI.Data;
using HealthifyAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class UsuariosController : ControllerBase
{
    private readonly AppDbContext _context;

    public UsuariosController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
    {
        return await _context.Usuarios.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Usuario>> GetUsuario(int id)
    {
        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario == null) return NotFound();
        return usuario;
    }

    [HttpPost]
    public async Task<ActionResult<Usuario>> PostUsuario([FromBody] Usuario usuario)
    {
        ModelState.Remove("Cliente");
        ModelState.Remove("Nutricionista");
        ModelState.Remove("Usuario");

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            if (usuario == null ||
                string.IsNullOrWhiteSpace(usuario.Nome) ||
                string.IsNullOrWhiteSpace(usuario.Email) ||
                string.IsNullOrWhiteSpace(usuario.senha) ||
                string.IsNullOrWhiteSpace(usuario.TipoUsuario) ||
                string.IsNullOrWhiteSpace(usuario.cpf) ||
                string.IsNullOrWhiteSpace(usuario.telefone) ||
                usuario.DataNascimento == DateTime.MinValue ||
                string.IsNullOrWhiteSpace(usuario.Sexo) ||
                string.IsNullOrWhiteSpace(usuario.Endereco))
            {
                return BadRequest("Dados inválidos. Certifique-se de que 'nome', 'email', 'senha', 'CPF', 'telefone', 'dataNascimento', 'sexo' e 'endereco' estão preenchidos.");
            }

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            if (usuario.TipoUsuario == "Cliente")
            {
                var cliente = new Cliente
                {
                    UsuarioId = usuario.UsuarioId,
                    Peso = null,
                    Altura = null,
                    Objetivo = null,
                    NivelAtividade = null,
                    PreferenciasAlimentares = null,
                    DoencasPreexistentes = null
                };
                _context.Clientes.Add(cliente);
                await _context.SaveChangesAsync();
            }
            else if (usuario.TipoUsuario == "Nutricionista")
            {
                var nutricionista = new Nutricionista
                {
                    UsuarioId = usuario.UsuarioId,
                    Especialidade = null,
                    Descricao = null
                };
                _context.Nutricionistas.Add(nutricionista);
                await _context.SaveChangesAsync();
            }
            else
            {
                return BadRequest("Tipo de usuário inválido. Use 'Cliente' ou 'Nutricionista'.");
            }

            return CreatedAtAction(nameof(GetUsuario), new { id = usuario.UsuarioId }, usuario);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro interno: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Detalhes do erro interno: {ex.InnerException.Message}");
                Console.WriteLine($"Stack trace do erro interno: {ex.InnerException.StackTrace}");
            }
            return StatusCode(500, $"Erro interno: {ex.Message}");
        }
    }

    public class LoginRequest
    {
        public string? Email { get; set; }
        public string? Senha { get; set; }
    }

    [HttpPost("login")]
    public async Task<ActionResult<Usuario>> Login([FromBody] LoginRequest loginRequest)
    {
        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Email == loginRequest.Email && u.senha == loginRequest.Senha);

        if (usuario == null)
        {
            return Unauthorized("Email ou senha inválidos.");
        }

        return Ok(usuario);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutUsuario(int id, Usuario usuario)
    {
        if (id != usuario.UsuarioId)
            return BadRequest();

        _context.Entry(usuario).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUsuario(int id)
    {
        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario == null)
            return NotFound();

        _context.Usuarios.Remove(usuario);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // Rota nova: GET /api/usuarios/logado
    [HttpGet("logado")]
    public async Task<ActionResult<Usuario>> GetUsuarioLogado()
    {
        try
        {
            // Aqui você deve implementar a lógica de autenticação real,
            // pegar o usuário logado pela sessão, token, etc.
            // Exemplo fixo (para teste), retorna usuário com ID 1:
            int usuarioLogadoId = 1;

            var usuario = await _context.Usuarios.FindAsync(usuarioLogadoId);

            if (usuario == null) return NotFound("Usuário logado não encontrado.");

            return Ok(usuario);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro ao buscar usuário logado: {ex.Message}");
        }
    }
}
