using System.ComponentModel.DataAnnotations;

namespace ControleGastos.Application.DTOs
{
    public class UsuarioDto
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string AzureAdId { get; set; } = string.Empty;
    }

    public class CreateUsuarioDto
    {
        [Required]
        [MaxLength(100)]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string AzureAdId { get; set; } = string.Empty;
    }

    public class UpdateUsuarioDto
    {
        [Required]
        [MaxLength(100)]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
