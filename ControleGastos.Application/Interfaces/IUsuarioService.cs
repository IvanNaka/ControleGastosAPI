using ControleGastos.Application.DTOs;

namespace ControleGastos.Application.Interfaces
{
    public interface IUsuarioService
    {
        Task<UsuarioDto?> GetByAzureAdIdAsync(string azureAdId);
        Task<UsuarioDto> CreateAsync(CreateUsuarioDto createDto);
        Task<bool> DeleteAsync(Guid id);
    }
}
