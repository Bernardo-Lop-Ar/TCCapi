namespace HealthifyAPI.Models.DTOs{
public class CriarPlanoDTO
{
    public int ClienteId { get; set; }
    public int NutricionistaId { get; set; }
    public string NomePlano { get; set; } = string.Empty;
    public DateTime? DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
    public string? Observacoes { get; set; }
    public List<PlanoReceitaDTO> Receitas { get; set; } = new();
}

public class PlanoReceitaDTO
{
    public int ReceitaId { get; set; }
    public int QuantidadePorcao { get; set; }
    public string DiaSemana { get; set; } = string.Empty;
    public string Refeicao { get; set; } = string.Empty;
}

}