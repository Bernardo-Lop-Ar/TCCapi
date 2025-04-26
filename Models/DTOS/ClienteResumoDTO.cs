namespace HealthifyAPI.Models.DTOs
{
    public class ClienteResumoDTO
    {
        // Dados do Cliente
        public int ClienteId { get; set; }
        public decimal? Peso { get; set; }
        public decimal? Altura { get; set; }
        public string? Objetivo { get; set; }
        public string? NivelAtividade { get; set; }
        public string? PreferenciasAlimentares { get; set; }
        public string? DoencasPreexistentes { get; set; }

        // Dados do Usuario (sem a senha)
        public int UsuarioId { get; set; }
        public string? Nome { get; set; }
        public string? Email { get; set; }
        public string? TipoUsuario { get; set; }
        public string? cpf { get; set; }
        public string? telefone { get; set; }
        public DateTime DataNascimento { get; set; }
        public string? Sexo { get; set; }
        public string? Endereco { get; set; }
    }
}
