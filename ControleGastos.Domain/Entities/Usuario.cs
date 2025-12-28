using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ControleGastos.Domain.Entities
{
    public class Usuario : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Nome { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [JsonIgnore]
        public string AzureAdId { get; set; }

        [JsonIgnore]
        public virtual ICollection<Pessoa> Pessoas { get; set; }
    }
}
