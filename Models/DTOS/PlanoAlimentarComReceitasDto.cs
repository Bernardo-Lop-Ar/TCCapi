using System.Collections.Generic;
using System;

namespace HealthifyAPI.Models.DTOs
{
    // Este é o "molde" para cada receita dentro do plano
    public class PlanoReceitaDto
    {
        public int ReceitaId { get; set; }
        public int QuantidadePorcao { get; set; }
        public string DiaSemana { get; set; }
        public string Refeicao { get; set; }
    }

    // Este é o "molde" principal que a sua API irá receber
    public class PlanoAlimentarComReceitasDto
    {
        public int ClienteId { get; set; }
        public int NutricionistaId { get; set; }
        public string NomePlano { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public string? Observacoes { get; set; }
        
        // A lista de receitas que vem junto com o plano
        public List<PlanoReceitaDto> Receitas { get; set; }
    }
}
