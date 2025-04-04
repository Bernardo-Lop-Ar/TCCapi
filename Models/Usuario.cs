namespace HealthifyAPI.Models
{
    public class Usuario
    {
        public int UsuarioId { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string SenhaHash { get; set; }
        public string TipoUsuario { get; set; }
        public DateTime DataCadastro { get; set; } = DateTime.Now;
        public DateTime? DataNascimento { get; set; }
        public string Sexo { get; set; }
        public string Telefone { get; set; }
        public string Endereco { get; set; }
        public string FotoPerfil { get; set; }
        public bool Ativo { get; set; } = true;
    }
}
