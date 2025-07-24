using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace HealthifyAPI.Models
{
    public class Nutricionista
    {
        [Key]
        public int NutricionistaId { get; set; }

        public int UsuarioId { get; set; }
        
        [ForeignKey("UsuarioId")]

        public Usuario? Usuario { get; set; } = null!;

        public string? Especialidade { get; set; }

        public string? Descricao { get; set; }

        public string? FotoPerfil { get; set; }
    }
}
