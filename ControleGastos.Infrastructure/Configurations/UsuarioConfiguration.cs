using ControleGastos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControleGastos.Infrastructure.Configurations
{
    public class UsuarioConfiguration : BaseConfiguration<Usuario>
    {
        public override void Configure(EntityTypeBuilder<Usuario> builder)
        {
            base.Configure(builder); // Apply base configuration

            builder.ToTable("Usuarios");

            builder.HasKey(prop => prop.Id);
            
            builder.Property(v => v.Id)
                .HasColumnName("Id")
                .ValueGeneratedOnAdd();
            
            builder.Property(v => v.Nome)
                .HasColumnName("Nome")
                .HasMaxLength(100)
                .IsRequired();
            
            builder.Property(v => v.Email)
                .HasColumnName("Email")
                .HasMaxLength(256)
                .IsRequired();

            builder.HasIndex(v => v.Email)
                .IsUnique();
            
            builder.Property(v => v.AzureAdId)
                .HasColumnName("AzureAdId")
                .HasMaxLength(100)
                .IsRequired();

            builder.HasIndex(v => v.AzureAdId)
                .IsUnique();

            // Relacionamento com Pessoas
            builder.HasMany(x => x.Pessoas)
                .WithOne(p => p.Usuario)
                .HasForeignKey(p => p.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
