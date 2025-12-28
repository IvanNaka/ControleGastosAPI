using ControleGastos.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;


namespace ControleGastos.Domain.Entities
{
    public class Categoria : BaseEntity
    {
        [Required]
        [MaxLength(255)]
        public string Descricao { get; set; }

        [Required]
        public FinalidadeCategoria Finalidade { get; set; } 

        // Chave Estrangeira para Usuário
        [Required]
        public Guid UsuarioId { get; set; }
        
        [JsonIgnore]
        public virtual Usuario Usuario { get; set; }

        // Relacionamento
        [JsonIgnore]
        public virtual ICollection<Transacao> Transacoes { get; set; }
    }
}
