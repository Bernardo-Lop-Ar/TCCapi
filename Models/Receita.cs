using System.ComponentModel.DataAnnotations;

namespace HealthifyAPI.Models
{
    public class Receita
{
    public int ReceitaId { get; set; }

    [Required(ErrorMessage = "O campo Nome é obrigatório.")]
    public string Nome { get; set; }

    [Required(ErrorMessage = "O campo Ingredientes é obrigatório.")]
    public string Ingredientes { get; set; }

    [Required(ErrorMessage = "O campo Instruções é obrigatório.")]
    public string Instrucoes { get; set; }

    [Required(ErrorMessage = "O campo Calorias por Porção é obrigatório.")]
    public double? CaloriasPorPorcao { get; set; }

    public string Categoria { get; set; }

    public string Tipo { get; set; }

    public string FotoReceita { get; set; } // Se for opcional, pode ser null
}


}
