using ControleGastos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControleGastos.Infrastructure.Configurations
{
    public class PessoaConfiguration : BaseConfiguration<Pessoa>
    {
        public override void Configure(EntityTypeBuilder<Pessoa> builder)
        {
            base.Configure(builder); // Apply base configuration

            builder.ToTable("Pessoas");

            builder.HasKey(prop => prop.Id);
            
            builder.Property(v => v.Nome)
                .HasColumnName("Nome")
                .HasMaxLength(155)
                .IsRequired();
            
            builder.Property(v => v.Idade)
                .HasColumnName("Idade")
                .IsRequired();
            
            builder.Property(v => v.UsuarioId)
                .HasColumnName("UsuarioId")
                .IsRequired();

            // Relacionamento com Usuario
            builder.HasOne(x => x.Usuario)
                .WithMany(u => u.Pessoas)
                .HasForeignKey(x => x.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relacionamento com Transacoes
            builder.HasMany(x => x.Transacoes)
                .WithOne(t => t.Pessoa)
                .HasForeignKey(t => t.PessoaId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
