using Microsoft.EntityFrameworkCore;
using HealthifyAPI.Models;

namespace HealthifyAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; } = null!;
        public DbSet<Cliente> Clientes { get; set; } = null!;
        public DbSet<Nutricionista> Nutricionistas { get; set; } = null!;
        public DbSet<Consulta> Consultas { get; set; } = null!;
        public DbSet<PlanoAlimentar> PlanosAlimentares { get; set; } = null!;
        public DbSet<Receita> Receitas { get; set; } = null!;
        public DbSet<PlanoReceita> PlanoReceitas { get; set; } = null!;
        public DbSet<ProgressoCliente> ProgressoCliente { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PlanoReceita>()
               .HasKey(pr => new { pr.PlanoId, pr.ReceitaId });

            base.OnModelCreating(modelBuilder);
        }
    }
}
