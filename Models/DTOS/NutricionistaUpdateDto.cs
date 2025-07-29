
using System.ComponentModel.DataAnnotations;

namespace HealthifyAPI.Models.DTOs
{
    public class NutricionistaUpdateDto
    {
        // IDs são importantes para o backend saber quais registos atualizar
        [Required]
        public int NutricionistaId { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        public string? Crn { get; set; }

        public string? Especialidade { get; set; }

        public string? Descricao { get; set; }
    }
}
