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
    // Opcional: estas linhas removem a validação de propriedades de navegação,
    // o que é bom para evitar erros inesperados de validação. Pode manter.
    ModelState.Remove("Cliente");
    ModelState.Remove("Nutricionista");

    // Validação básica do modelo
    if (!ModelState.IsValid)
    {
        return BadRequest(ModelState);
    }
    
    // Suas validações manuais de campos obrigatórios (pode manter se preferir,
    // mas a validação de modelo com [Required] no seu Model é mais limpa)
    if (usuario == null || string.IsNullOrWhiteSpace(usuario.Nome) ||
        string.IsNullOrWhiteSpace(usuario.Email) || string.IsNullOrWhiteSpace(usuario.senha))
    {
        return BadRequest("Dados de usuário inválidos.");
    }

    try
    {
        // --- LÓGICA CORRIGIDA ---
        // 1. Adicionamos o usuário ao contexto
        _context.Usuarios.Add(usuario);

        // 2. Salvamos as alterações para gerar o UsuarioId.
        //    NÃO criamos mais Cliente ou Nutricionista aqui.
        await _context.SaveChangesAsync(); 

        // 3. Retornamos os dados do usuário criado. O front-end usará o ID
        //    para fazer a segunda chamada ao controller correto (Clientes ou Nutricionistas).
        var resultado = new
        {
            usuario.UsuarioId,
            usuario.Nome,
            usuario.Email,
            usuario.TipoUsuario
        };

        return CreatedAtAction(nameof(GetUsuario), new { id = usuario.UsuarioId }, resultado);
    }
    catch (DbUpdateException ex) // Ser mais específico no erro de duplicidade
    {
        // Verifica se o erro é de chave duplicada (Ex: email ou CPF já existem)
        if (ex.InnerException != null && ex.InnerException.Message.Contains("Duplicate entry"))
        {
            return Conflict($"Erro de duplicação: {ex.InnerException.Message}");
        }
        Console.WriteLine($"Erro de banco de dados: {ex.InnerException?.Message ?? ex.Message}");
        return StatusCode(500, $"Erro de banco de dados: {ex.InnerException?.Message ?? ex.Message}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erro interno: {ex.Message}");
        return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
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
public async Task<IActionResult> DeleteCliente(int id)
{
    // 1. Encontra o cliente que queremos deletar.
    //    Usamos 'Include' para já trazer o objeto 'Usuario' associado.
    var clienteParaDeletar = await _context.Clientes
        .Include(c => c.Usuario) 
        .FirstOrDefaultAsync(c => c.ClienteId == id);

    if (clienteParaDeletar == null)
    {
        return NotFound("Cliente não encontrado.");
    }

    // Usamos uma transação para garantir que todas as operações
    // sejam concluídas com sucesso, ou nenhuma delas.
    using (var transaction = await _context.Database.BeginTransactionAsync())
    {
        try
        {
            // 2. Encontra e remove todos os registros que dependem do Cliente.
            //    Você PRECISA adicionar aqui todas as tabelas que usam 'ClienteId'.
            
            // Exemplo para QuestionarioRespostas
            var respostas = _context.QuestionarioRespostas.Where(qr => qr.ClienteId == id);
            _context.QuestionarioRespostas.RemoveRange(respostas);

            // Exemplo para PlanosAlimentares (ADICIONE SE TIVER)
            // var planos = _context.PlanosAlimentares.Where(p => p.ClienteId == id);
            // _context.PlanosAlimentares.RemoveRange(planos);

            // Exemplo para Consultas (ADICIONE SE TIVER)
            // var consultas = _context.Consultas.Where(c => c.ClienteId == id);
            // _context.Consultas.RemoveRange(consultas);

            // 3. Agora que os "netos" foram removidos, removemos o "filho" (Cliente).
            _context.Clientes.Remove(clienteParaDeletar);

            // 4. Por fim, removemos o "pai" (Usuario), se ele existir.
            if (clienteParaDeletar.Usuario != null)
            {
                _context.Usuarios.Remove(clienteParaDeletar.Usuario);
            }

            // 5. Salva todas as alterações de uma vez.
            await _context.SaveChangesAsync();

            // 6. Confirma a transação, tornando as exclusões permanentes.
            await transaction.CommitAsync();

            // Retorna 204 No Content, indicando sucesso na exclusão.
            return NoContent();
        }
        catch (Exception ex)
        {
            // Se algo der errado, desfaz todas as operações.
            await transaction.RollbackAsync();
            // Log do erro para depuração
            Console.WriteLine($"Erro ao excluir cliente em cascata: {ex.Message}");
            return StatusCode(500, "Ocorreu um erro interno ao tentar excluir o cliente e seus dados associados.");
        }
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


