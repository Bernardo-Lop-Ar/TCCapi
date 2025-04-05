using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthifyAPI.Models
{
    public class ProgressoCliente
    {
        [Key]
        public int ProgressoId { get; set; }

        public int ClienteId { get; set; }

        [ForeignKey("ClienteId")]
        public Cliente Cliente { get; set; } = null!;

        public DateTime DataRegistro { get; set; } = DateTime.Now;

        public decimal? Peso { get; set; }

        public decimal? IMC { get; set; }

        public string? Medidas { get; set; }

        public string? Observacoes { get; set; }
    }
}
