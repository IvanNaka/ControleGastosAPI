using Microsoft.EntityFrameworkCore;
using ControleGastos.Domain.Entities;

namespace ControleGastos.Infrastructure
{
    public class ControleGastosDbContext : DbContext
    {
        public ControleGastosDbContext(DbContextOptions<ControleGastosDbContext> options) : base(options)
        {
        }
        
        public DbSet<Pessoa> Pessoas { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Transacao> Transacoes { get; set; }
        public DbSet<Categoria> Categorias { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(ControleGastosDbContext).Assembly);

            // Filtro para trazer apenas ativos
            builder.Entity<Pessoa>().HasQueryFilter(e => e.Ativo == true);
            builder.Entity<Usuario>().HasQueryFilter(e => e.Ativo == true);
            builder.Entity<Transacao>().HasQueryFilter(e => e.Ativo == true);
            builder.Entity<Categoria>().HasQueryFilter(e => e.Ativo == true);
        }
    }
}
