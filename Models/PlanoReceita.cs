using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthifyAPI.Models
{
    public class PlanoReceita
    {
        [Key, Column(Order = 0)]
        public int PlanoId { get; set; }

        [Key, Column(Order = 1)]
        public int ReceitaId { get; set; }

        [Key, Column(Order = 2)]
        public string DiaSemana { get; set; } = null!; // segunda, terca, etc.

        [Key, Column(Order = 3)]
        public string Refeicao { get; set; } = null!; // cafe, almoco, jantar, etc.

        public int QuantidadePorcao { get; set; }

        [ForeignKey("PlanoId")]
        public PlanoAlimentar PlanoAlimentar { get; set; } = null!;

        [ForeignKey("ReceitaId")]
        public Receita Receita { get; set; } = null!;
    }
}
