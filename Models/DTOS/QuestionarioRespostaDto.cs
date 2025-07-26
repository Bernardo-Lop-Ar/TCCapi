

using System;
using System.ComponentModel.DataAnnotations;

namespace HealthifyAPI.Models.DTOs
{
    public class QuestionarioRespostaDto
    {
        [Required]
        public int ClienteId { get; set; }

        [Required]
        public int PerguntaId { get; set; }

        [Required]
        public string RespostaTexto { get; set; }

        [Required]
        public DateTime DataResposta { get; set; }
    }
}