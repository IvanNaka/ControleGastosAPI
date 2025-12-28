using ControleGastos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControleGastos.Infrastructure.Configurations
{
    public class CategoriaConfiguration : BaseConfiguration<Categoria>
    {
        public override void Configure(EntityTypeBuilder<Categoria> builder)
        {
            base.Configure(builder);

            builder.ToTable("Categorias");

            builder.HasKey(prop => prop.Id);
            
            builder.Property(v => v.Descricao)
                .HasColumnName("Descricao")
                .HasMaxLength(255)
                .IsRequired();
            
            builder.Property(v => v.Finalidade)
                .HasColumnName("Finalidade")
                .IsRequired();
            
            builder.Property(v => v.UsuarioId)
                .HasColumnName("UsuarioId")
                .IsRequired();

            builder.HasOne(x => x.Usuario)
                .WithMany()
                .HasForeignKey(x => x.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Transacoes)
                .WithOne(t => t.Categoria)
                .HasForeignKey(t => t.CategoriaId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}