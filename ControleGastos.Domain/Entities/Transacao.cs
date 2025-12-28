using ControleGastos.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ControleGastos.Domain.Entities
{
    public class Transacao : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string Descricao { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")] 
        public decimal Valor { get; set; }

        [Required]
        public TipoTransacao Tipo { get; set; }

        [Required]
        public Guid PessoaId { get; set; }
        public virtual Pessoa Pessoa { get; set; }

        [Required]
        public Guid CategoriaId { get; set; }
        public virtual Categoria Categoria { get; set; }

        [Required]
        public Guid UsuarioId { get; set; }
        [JsonIgnore]
        public virtual Usuario Usuario { get; set; }
    }
}
