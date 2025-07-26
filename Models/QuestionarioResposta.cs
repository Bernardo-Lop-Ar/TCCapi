

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthifyAPI.Models
{
    public class QuestionarioResposta
    {
        [Key]
        public int QuestionarioRespostaId { get; set; }

        [Required]
        public int ClienteId { get; set; }

        [ForeignKey("ClienteId")]
        public virtual Cliente Cliente { get; set; }

        [Required]
        public int PerguntaId { get; set; } // Propriedade que estava faltando

        [ForeignKey("PerguntaId")]
        public virtual Pergunta Pergunta { get; set; }

        [Required]
        public string RespostaTexto { get; set; } // Propriedade que estava faltando

        [Required]
        public DateTime DataResposta { get; set; } // Propriedade que estava faltando
    }
}