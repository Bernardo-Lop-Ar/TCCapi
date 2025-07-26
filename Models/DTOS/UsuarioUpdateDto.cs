

using System;
using System.ComponentModel.DataAnnotations;

namespace HealthifyAPI.Models.DTOs
{
    public class UsuarioUpdateDto
    {
        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(100)]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O email é obrigatório.")]
        [EmailAddress(ErrorMessage = "Formato de email inválido.")]
        public string Email { get; set; }

        public string? Telefone { get; set; }
        
        public string? Endereco { get; set; }

        public string? Sexo { get; set; }

        public DateTime? DataNascimento { get; set; }


    }
}