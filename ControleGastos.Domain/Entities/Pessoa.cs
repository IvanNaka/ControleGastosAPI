using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ControleGastos.Domain.Entities
{
    public class Pessoa : BaseEntity
    {
        [Required]
        [MaxLength(155)]
        public string Nome { get; set; }

        [Required]
        public int Idade { get; set; }

        [Required]
        public Guid UsuarioId { get; set; }
        [JsonIgnore]
        public virtual Usuario Usuario { get; set; }

        [JsonIgnore]
        public virtual ICollection<Transacao> Transacoes { get; set; }
    }
}
