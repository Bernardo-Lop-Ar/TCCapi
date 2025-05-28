using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
namespace HealthifyAPI.Models
{
    public class Cliente
    {
        [Key]
        public int ClienteId { get; set; }

        public int UsuarioId { get; set; }
        
        [JsonIgnore]
        [ForeignKey("UsuarioId")]
        [ValidateNever]
        public Usuario? Usuario { get; set; } 

        public decimal? Peso { get; set; }

        public decimal? Altura { get; set; }

        public string? Objetivo { get; set; }

        public string? NivelAtividade { get; set; }

        public string? PreferenciasAlimentares { get; set; }

        public string? DoencasPreexistentes { get; set; }
    }
}
