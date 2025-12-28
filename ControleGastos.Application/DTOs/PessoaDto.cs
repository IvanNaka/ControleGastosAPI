using System.ComponentModel.DataAnnotations;

namespace ControleGastos.Application.DTOs
{
    public class PessoaDto
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(155)]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [Range(0, 150)]
        public int Idade { get; set; }

        [Required]
        public Guid UsuarioId { get; set; }
    }

    public class CreatePessoaDto
    {
        [Required]
        [MaxLength(155)]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [Range(0, 150)]
        public int Idade { get; set; }

        public Guid UsuarioId { get; set; }
    }

    public class UpdatePessoaDto
    {
        [Required]
        [MaxLength(155, ErrorMessage = "Nome não pode exceder 155 caracteres")]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [Range(0, 150, ErrorMessage = "Idade deve estar entre 0 e 150")]
        public int Idade { get; set; }
    }
}
