using System;
using System.ComponentModel.DataAnnotations;

namespace HealthifyAPI.Models.DTOs
{
    public class ConsultaDTO
    {
        public int ConsultaId { get; set; }

        [Required]
        public int ClienteId { get; set; }

        public ClienteDTO? Cliente { get; set; } 

        [Required]
        public int NutricionistaId { get; set; }

        public NutricionistaDTO? Nutricionista { get; set; } 

        [Required]
        public string HoraConsulta { get; set; } = "";

        [Required]
        public DateTime DataConsulta { get; set; } 

        public string? TipoConsulta { get; set; }

        public string? Status { get; set; }

        public string? Observacoes { get; set; }
    }
}
