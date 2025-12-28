using ControleGastos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControleGastos.Infrastructure.Configurations
{
    public class TransacaoConfiguration : BaseConfiguration<Transacao>
    {
        public override void Configure(EntityTypeBuilder<Transacao> builder)
        {
            base.Configure(builder); // Apply base configuration

            builder.ToTable("Transacoes");

            builder.HasKey(prop => prop.Id);
            
            builder.Property(v => v.Descricao)
                .HasColumnName("Descricao")
                .HasMaxLength(200)
                .IsRequired();
            
            builder.Property(v => v.Valor)
                .HasColumnName("Valor")
                .HasColumnType("decimal(18,2)")
                .IsRequired();
            
            builder.Property(v => v.Tipo)
                .HasColumnName("Tipo")
                .IsRequired();
            
            builder.Property(v => v.PessoaId)
                .HasColumnName("PessoaId")
                .IsRequired();
            
            builder.Property(v => v.CategoriaId)
                .HasColumnName("CategoriaId")
                .IsRequired();
            
            builder.Property(v => v.UsuarioId)
                .HasColumnName("UsuarioId")
                .IsRequired();

            // Relacionamento com Pessoa
            builder.HasOne(x => x.Pessoa)
                .WithMany(p => p.Transacoes)
                .HasForeignKey(x => x.PessoaId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relacionamento com Categoria
            builder.HasOne(x => x.Categoria)
                .WithMany(c => c.Transacoes)
                .HasForeignKey(x => x.CategoriaId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relacionamento com Usuario
            builder.HasOne(x => x.Usuario)
                .WithMany()
                .HasForeignKey(x => x.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
