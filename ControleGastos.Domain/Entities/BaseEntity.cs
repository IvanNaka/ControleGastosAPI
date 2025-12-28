using System;
using System.ComponentModel.DataAnnotations;

namespace ControleGastos.Domain.Entities
{
    public abstract class BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
        public DateTime? UltimaAtualizacao { get; set; }
        public bool? Ativo { get; private set; } = true;
        
        public virtual void Ativar()
            => Ativo = true;

        public virtual void Deletar()
            => Ativo = false;
    }
}
