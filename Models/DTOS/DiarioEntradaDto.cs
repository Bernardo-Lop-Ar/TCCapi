using System;
using System.ComponentModel.DataAnnotations;

namespace HealthifyAPI.Models.DTOs
{
    public class DiarioEntradaDto
    {
        [Required]
        public int ClienteId { get; set; }

        [Required]
        public DateTime DataEntrada { get; set; }

        public string? Refeicoes { get; set; }

        public string? Sintomas { get; set; }
    }
}