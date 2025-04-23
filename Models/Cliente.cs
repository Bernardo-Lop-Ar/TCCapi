using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthifyAPI.Models
{
    public class Cliente
    {
        [Key]
        public int ClienteId { get; set; }

        public int UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public Usuario? Usuario { get; set; } = null!;

        public decimal? Peso { get; set; }

        public decimal? Altura { get; set; }

        public string? Objetivo { get; set; }

        public string? NivelAtividade { get; set; }

        public string? PreferenciasAlimentares { get; set; }

        public string? DoencasPreexistentes { get; set; }
    }
}
