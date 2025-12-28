using ControleGastos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControleGastos.Infrastructure.Configurations
{
    public abstract class BaseConfiguration<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : BaseEntity
    {
        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
            builder.Property(prop => prop.DataCriacao).HasColumnType("datetime").HasColumnName("DataCriacao").IsRequired();
            builder.Property(prop => prop.UltimaAtualizacao).HasColumnType("datetime").HasColumnName("UltimaAtualizacao").IsRequired(false);
            builder.Property(prop => prop.Ativo).HasColumnName("Ativo").HasDefaultValue(true).IsRequired();
        }
    }
}
