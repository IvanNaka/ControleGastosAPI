using System.ComponentModel.DataAnnotations;
using ControleGastos.Domain.Enums;

namespace ControleGastos.Application.DTOs
{
    public class TransacaoDto
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Descricao { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Valor { get; set; }

        [Required]
        public TipoTransacao Tipo { get; set; }

        [Required]
        public Guid PessoaId { get; set; }

        public string? PessoaNome { get; set; }

        [Required]
        public Guid CategoriaId { get; set; }

        public string? CategoriaDescricao { get; set; }

        [Required]
        public Guid UsuarioId { get; set; }
    }

    public class CreateTransacaoDto
    {
        [Required]
        [MaxLength(200)]
        public string Descricao { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Valor { get; set; }

        [Required]
        public TipoTransacao Tipo { get; set; }

        [Required]
        public Guid PessoaId { get; set; }

        [Required]
        public Guid CategoriaId { get; set; }

        [Required]
        public Guid UsuarioId { get; set; }
    }

    public class UpdateTransacaoDto
    {
        [Required]
        [MaxLength(200)]
        public string Descricao { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Valor { get; set; }

        [Required]
        public TipoTransacao Tipo { get; set; }

        [Required]
        public Guid PessoaId { get; set; }

        [Required]
        public Guid CategoriaId { get; set; }
    }
}
