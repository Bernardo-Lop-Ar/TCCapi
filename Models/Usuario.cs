using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HealthifyAPI.Models
{
    public class Usuario
    {
        [Key]
        public int UsuarioId { get; set; }

        [Required]
        [JsonPropertyName("nome")]
        public string? Nome { get; set; }

        [Required]
        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [Required]
        [JsonPropertyName("senha")]
        public string? SenhaHash { get; set; }
        
        [Required]
        public string? TipoUsuario { get; set; }
    }
}
