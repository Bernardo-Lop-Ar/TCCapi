using System;
using System.ComponentModel.DataAnnotations;

namespace HealthifyAPI.Models.DTOs
{
    public class ConsultaDTO
    {
        public int ConsultaId { get; set; } // ID da consulta

        [Required]
        public int ClienteId { get; set; }

        public ClienteDTO? Cliente { get; set; } // Dados do cliente (opcional)

        [Required]
        public int NutricionistaId { get; set; }

        public NutricionistaDTO? Nutricionista { get; set; } // Dados do nutricionista (opcional)

        [Required]
        public string HoraConsulta { get; set; } = "";

        [Required]
        public DateTime DataConsulta { get; set; } 

        public string? TipoConsulta { get; set; }

        public string? Status { get; set; }

        public string? Observacoes { get; set; }
    }
}
