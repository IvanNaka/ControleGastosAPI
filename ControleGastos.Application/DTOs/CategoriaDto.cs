using System.ComponentModel.DataAnnotations;
using ControleGastos.Domain.Enums;

namespace ControleGastos.Application.DTOs
{
    public class CategoriaDto
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Descricao { get; set; } = string.Empty;

        [Required]
        public FinalidadeCategoria Finalidade { get; set; }

        [Required]
        public Guid UsuarioId { get; set; }

        public DateTime DataCriacao { get; set; }
        public DateTime? UltimaAtualizacao { get; set; }
        public bool? Ativo { get; set; }
    }

    public class CreateCategoriaDto
    {
        [Required]
        [MaxLength(255)]
        public string Descricao { get; set; } = string.Empty;

        [Required]
        public FinalidadeCategoria Finalidade { get; set; }

        [Required]
        public Guid UsuarioId { get; set; }
    }

    public class UpdateCategoriaDto
    {
        [Required]
        [MaxLength(255)]
        public string Descricao { get; set; } = string.Empty;

        [Required]
        public FinalidadeCategoria Finalidade { get; set; }
    }
}
