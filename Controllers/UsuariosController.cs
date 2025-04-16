using HealthifyAPI.Data;
using HealthifyAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;

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
    try
    {
        // Validação dos campos principais
        if (usuario == null ||
            string.IsNullOrWhiteSpace(usuario.Nome) ||
            string.IsNullOrWhiteSpace(usuario.Email) ||
            string.IsNullOrWhiteSpace(usuario.senha) ||
            string.IsNullOrWhiteSpace(usuario.TipoUsuario) ||
            string.IsNullOrWhiteSpace(usuario.cpf) ||
            string.IsNullOrWhiteSpace(usuario.telefone) ||
            usuario.DataNascimento == null ||    
            string.IsNullOrWhiteSpace(usuario.Sexo) || // Verificando se o sexo foi preenchido
            string.IsNullOrWhiteSpace(usuario.Endereco)) // Verificando se o endereço foi preenchido
        {
            return BadRequest("Dados inválidos. Certifique-se de que 'nome', 'email', 'senha', 'CPF', 'telefone', 'dataNascimento', 'sexo' e 'endereco' estão preenchidos.");
        }

        // Adicionar o usuário à base de dados
        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        // Salvar dados adicionais de acordo com o tipo de usuário
        if (usuario.TipoUsuario == "Cliente")
        {
            var cliente = new Cliente
            {
                UsuarioId = usuario.UsuarioId,
                Peso = null,  // Inicializando como null ou configurado para ser recebido via API
                Altura = null,
                Objetivo = null,
                NivelAtividade = null,
                PreferenciasAlimentares = null,
                DoencasPreexistentes = null
            };
            _context.Clientes.Add(cliente);
        }
        else if (usuario.TipoUsuario == "Nutricionista")
        {
            var nutricionista = new Nutricionista
            {
                UsuarioId = usuario.UsuarioId,
                Especialidade = null, // Inicializando como null ou configurado para ser recebido via API
                Descricao = null
            };
            _context.Nutricionistas.Add(nutricionista);
        }

        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetUsuario), new { id = usuario.UsuarioId }, usuario);
    }
    catch (Exception ex)
    {
        return StatusCode(500, $"Erro interno: {ex.Message}");
    }
}


    [HttpPost("login")]
    public async Task<ActionResult<Usuario>> Login([FromBody] JsonElement json)
    {
        string? email = json.GetProperty("email").GetString();
        string? senha = json.GetProperty("senha").GetString();

        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Email == email && u.senha == senha);

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
}
