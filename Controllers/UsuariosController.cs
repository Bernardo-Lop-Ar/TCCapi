using HealthifyAPI.Data;
using HealthifyAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using HealthifyAPI.Models.DTOs;

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

        // A sua validação inicial está correta e pode ser mantida.
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            // A sua lógica para verificar os campos também está correta.
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
                return BadRequest("Dados inválidos. Certifique-se de que todos os campos estão preenchidos.");
            }

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync(); // Salva o utilizador para que ele receba um ID

            // A sua lógica para criar Cliente ou Nutricionista está correta.
            if (usuario.TipoUsuario == "Cliente")
            {
                var cliente = new Cliente { UsuarioId = usuario.UsuarioId };
                _context.Clientes.Add(cliente);
                await _context.SaveChangesAsync();
            }
            else if (usuario.TipoUsuario == "Nutricionista")
            {
                var nutricionista = new Nutricionista { UsuarioId = usuario.UsuarioId };
                _context.Nutricionistas.Add(nutricionista);
                await _context.SaveChangesAsync();
            }
            else
            {
                return BadRequest("Tipo de usuário inválido. Use 'Cliente' ou 'Nutricionista'.");
            }

            // --- CORREÇÃO PRINCIPAL AQUI ---
            // Em vez de retornar o objeto 'usuario' completo, que causa o ciclo,
            // retornamos um objeto anónimo simples com os dados essenciais.
            var resultado = new
            {
                usuario.UsuarioId,
                usuario.Nome,
                usuario.Email,
                usuario.TipoUsuario
            };

            return CreatedAtAction(nameof(GetUsuario), new { id = usuario.UsuarioId }, resultado);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro interno: {ex.InnerException?.Message ?? ex.Message}");
            return StatusCode(500, $"Erro interno: {ex.InnerException?.Message ?? ex.Message}");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutUsuario(int id, [FromBody] UsuarioUpdateDto usuarioDto)
    {
        // 1. Verifica se o DTO enviado é válido (ex: email com formato correto)
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // 2. Busca o usuário original no banco de dados
        var usuarioNoBanco = await _context.Usuarios.FindAsync(id);

        if (usuarioNoBanco == null)
        {
            return NotFound("Usuário não encontrado.");
        }

        // 3. Atualiza APENAS os campos recebidos do DTO.
        // Isso evita que a senha e outros dados sejam apagados!
        usuarioNoBanco.Nome = usuarioDto.Nome;
        usuarioNoBanco.Email = usuarioDto.Email;
        usuarioNoBanco.telefone = usuarioDto.Telefone;
        usuarioNoBanco.Endereco = usuarioDto.Endereco;
        usuarioNoBanco.Sexo = usuarioDto.Sexo;
        usuarioNoBanco.DataNascimento = usuarioDto.DataNascimento ?? usuarioNoBanco.DataNascimento; // Mantém a data antiga se nenhuma nova for enviada

        // 4. Salva as alterações
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            // Trata casos onde o usuário pode ter sido deletado enquanto era editado
            if (!_context.Usuarios.Any(e => e.UsuarioId == id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent(); // Retorna 204 No Content, indicando sucesso.
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

    // Rota protegida para obter o usuário logado via token JWT
    [Authorize]
    [HttpGet("logado")]
    public async Task<ActionResult<Usuario>> GetUsuarioLogado()
    {
        try
        {
            var usuarioIdClaim = User.FindFirst("UsuarioId");
            if (usuarioIdClaim == null)
                return Unauthorized("Usuário não autenticado.");

            if (!int.TryParse(usuarioIdClaim.Value, out int usuarioId))
                return Unauthorized("Usuário inválido.");

            var usuario = await _context.Usuarios.FindAsync(usuarioId);

            if (usuario == null) return NotFound("Usuário logado não encontrado.");

            return Ok(usuario);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro ao buscar usuário logado: {ex.Message}");
        }
    }

    // ROTA DE LOGIN
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UsuarioLoginRequest login)
    {
        if (string.IsNullOrEmpty(login.Email) || string.IsNullOrEmpty(login.Senha))
        {
            return BadRequest("Email e senha são obrigatórios.");
        }

        var usuario = await _context.Usuarios.FirstOrDefaultAsync(u =>
            u.Email == login.Email && u.senha == login.Senha);

        if (usuario == null)
        {
            return Unauthorized("Email ou senha inválidos.");
        }

        return Ok(usuario);
    }
    [HttpGet("Clientes/usuario/{usuarioId}")]
    public IActionResult GetClienteByUsuarioId(int usuarioId)
    {
        var cliente = _context.Clientes.FirstOrDefault(c => c.UsuarioId == usuarioId);
        if (cliente == null)
            return NotFound();

        return Ok(cliente);
    }
    [Authorize]
    [Authorize]
    [HttpGet("perfil")]
    public async Task<ActionResult<object>> GetPerfil()
    {
        try
        {
            var usuarioIdClaim = User.FindFirst("UsuarioId");
            if (usuarioIdClaim == null)
                return Unauthorized("Usuário não autenticado.");

            if (!int.TryParse(usuarioIdClaim.Value, out int usuarioId))
                return Unauthorized("Usuário inválido.");

            var usuario = await _context.Usuarios
                .Include(u => u.Cliente)
                .Include(u => u.Nutricionista)
                .FirstOrDefaultAsync(u => u.UsuarioId == usuarioId);

            if (usuario == null)
                return NotFound("Usuário não encontrado.");

            // Montar objeto perfil para retornar apenas os dados necessários
            var perfil = new
            {
                usuario.UsuarioId,
                usuario.Nome,
                usuario.Email,
                usuario.telefone,
                usuario.TipoUsuario,
                usuario.cpf,
                usuario.DataNascimento,
                usuario.Sexo,
                usuario.Endereco,

                // CORREÇÃO AQUI: Criamos um objeto anônimo para Cliente para quebrar o ciclo
                Cliente = usuario.Cliente == null ? null : new
                {
                    usuario.Cliente.ClienteId,
                    usuario.Cliente.Peso,
                    usuario.Cliente.Altura,
                    usuario.Cliente.Objetivo,
                    usuario.Cliente.NivelAtividade,
                    usuario.Cliente.PreferenciasAlimentares,
                    usuario.Cliente.DoencasPreexistentes
                },

                Nutricionista = usuario.Nutricionista == null ? null : new
                {
                    usuario.Nutricionista.NutricionistaId,
                    usuario.Nutricionista.Especialidade,
                    usuario.Nutricionista.Descricao
                }
            };

            return Ok(perfil);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro ao buscar perfil: {ex.Message}");
        }
    }


    public class UsuarioLoginRequest
    {
        public string Email { get; set; }
        public string Senha { get; set; }
    }
}


