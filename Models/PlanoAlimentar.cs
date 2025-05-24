using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace HealthifyAPI.Models
{
    public class PlanoAlimentar
    {
        public ICollection<PlanoReceita> PlanoReceita { get; set; } = new List<PlanoReceita>();

        [Key]
        public int PlanoId { get; set; }

        public int ClienteId { get; set; }

        public int NutricionistaId { get; set; }

        [ForeignKey("ClienteId")]
        public Cliente Cliente { get; set; } = null!;

        [ForeignKey("NutricionistaId")]
        public Nutricionista Nutricionista { get; set; } = null!;

        public string? NomePlano { get; set; }

        public DateTime DataCriacao { get; set; } = DateTime.Now;

        public DateTime? DataInicio { get; set; }

        public DateTime? DataFim { get; set; }

        public string? Observacoes { get; set; }
    }
}
