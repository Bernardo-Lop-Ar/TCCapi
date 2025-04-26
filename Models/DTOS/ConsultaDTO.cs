using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace HealthifyAPI.Models.DTOs
{
    public class ConsultaDTO
    {
        public int ConsultaId { get; set; } // Adicionado

        public int ClienteId { get; set; }
        public ClienteDTO? Cliente { get; set; } // Adicionado

        public int NutricionistaId { get; set; }
        public NutricionistaDTO? Nutricionista { get; set; } // Adicionado

        public DateTime DataConsulta { get; set; }
        public string? TipoConsulta { get; set; }
        public string? Status { get; set; }
        public string? Observacoes { get; set; }
    }
}
