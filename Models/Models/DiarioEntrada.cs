using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthifyAPI.Models
{
    public class DiarioEntrada
    {
        [Key]
        public int DiarioId { get; set; }

        [Required]
        public int ClienteId { get; set; }
        
        [ForeignKey("ClienteId")]
        public virtual Cliente Cliente { get; set; }

        [Required]
        public DateTime DataEntrada { get; set; }

        public string? Refeicoes { get; set; } // O que comeu

        public string? Sintomas { get; set; } // Como se sentiu
    }
}