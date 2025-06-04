using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HealthifyAPI.Data;
using System.Linq;

namespace HealthifyAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly AppDbContext _context;

        public AuthController(IConfiguration config, AppDbContext context)
        {
            _config = config;
            _context = context;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest login)
        {

            var usuario = _context.Usuarios
                .FirstOrDefault(u => u.Email == login.Email && u.senha == login.Senha);

            if (usuario == null)
                return Unauthorized("Credenciais inválidas");

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, usuario.Email),
                new Claim("UsuarioId", usuario.UsuarioId.ToString()),
                new Claim("TipoUsuario", usuario.TipoUsuario)
            };

            var keyString = _config["Jwt:Key"];
            if (string.IsNullOrEmpty(keyString))
            {
                return StatusCode(500, "Chave JWT não configurada.");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds);

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token)
            });
        }

        public record LoginRequest(string Email, string Senha);
    }
}
