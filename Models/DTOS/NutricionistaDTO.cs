using System.ComponentModel.DataAnnotations;

namespace HealthifyAPI.Models.DTOs
{
    public class NutricionistaDTO
    {
        public int NutricionistaId { get; set; }
        public int UsuarioId { get; set; }

        // Dados do usuário
        public string? Nome { get; set; }
        public string? Email { get; set; }
        public string? Telefone { get; set; }
        public string? Sexo { get; set; }
        public string? Cpf { get; set; }
        public string? Endereco { get; set; }
        public DateTime? DataNascimento { get; set; }

        // Dados do nutricionista
        public string? Especialidade { get; set; }
        public string? Descricao { get; set; }
        public string? FotoPerfil { get; set; }
    }
}
