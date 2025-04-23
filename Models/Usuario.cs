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
        public string? senha { get; set; }
        
        [Required]
        public string? TipoUsuario { get; set; }

        [Required]
        public string? cpf { get; set; }

        [Required]
        public string? telefone { get; set; }

        // Campos adicionais
        [Required]
        public DateTime DataNascimento { get; set; } // Data de Nascimento

        [Required]
        public string? Sexo { get; set; } // Sexo

        [Required]
        public string? Endereco { get; set; } // Endereço
    }
}
