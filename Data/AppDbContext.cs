using Microsoft.EntityFrameworkCore;
using HealthifyAPI.Models;

namespace HealthifyAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
       // public DbSet<Cliente> Clientes { get; set; }
        //public DbSet<Nutricionista> Nutricionistas { get; set; }
        //public DbSet<Consulta> Consultas { get; set; }
        //public DbSet<PlanoAlimentar> PlanosAlimentares { get; set; }
        //public DbSet<Receita> Receitas { get; set; }
        //public DbSet<PlanoReceita> PlanoReceitas { get; set; }
        //public DbSet<ProgressoCliente> ProgressoClientes { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<PlanoReceita>()
       //         .HasKey(pr => new { pr.PlanoId, pr.ReceitaId });

        //    base.OnModelCreating(modelBuilder);
        //}
    }
}
