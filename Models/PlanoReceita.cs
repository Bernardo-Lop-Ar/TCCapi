using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthifyAPI.Models
{
    public class PlanoReceita
    {
        public int PlanoId { get; set; }

        public int ReceitaId { get; set; }

        public int QuantidadePorcao { get; set; }

        [ForeignKey("PlanoId")]
        public PlanoAlimentar PlanoAlimentar { get; set; } = null!;

        [ForeignKey("ReceitaId")]
        public Receita Receita { get; set; } = null!;
    }
}
