

using System.ComponentModel.DataAnnotations;

namespace HealthifyAPI.Models
{
    public class Pergunta
    {
        [Key]
        public int PerguntaId { get; set; }

        [Required]
        public string Texto { get; set; }
    }
}