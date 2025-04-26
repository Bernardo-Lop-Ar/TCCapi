using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthifyAPI.Models
{
    public class Consulta
    {
        [Key]
        public int ConsultaId { get; set; }

        public int ClienteId { get; set; }

        public int NutricionistaId { get; set; }

        [ForeignKey("ClienteId")]
        public Cliente Cliente { get; set; } = null!;

        [ForeignKey("NutricionistaId")]
        public Nutricionista Nutricionista { get; set; } = null!;

        [Required]
        public DateTime DataConsulta { get; set; }

        [Required]
        public string? TipoConsulta { get; set; }

        public string? Status { get; set; } = "Agendada";

        public string? Observacoes { get; set; }
    }
}
