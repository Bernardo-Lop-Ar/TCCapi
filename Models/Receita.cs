using System.ComponentModel.DataAnnotations;

namespace HealthifyAPI.Models
{
    public class Receita
    {
        [Key]
        public int ReceitaId { get; set; }

        public string? Nome { get; set; }

        public string? Ingredientes { get; set; }

        public string? Instrucoes { get; set; }

        public decimal? CaloriasPorPorcao { get; set; }

        public string? Categoria { get; set; }

        public string? Tipo { get; set; }

        public string? FotoReceita { get; set; }
    }
}
