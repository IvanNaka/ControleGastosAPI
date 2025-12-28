using ControleGastos.Application.DTOs;

namespace ControleGastos.Application.Interfaces
{
    public interface ICategoriaService
    {
        Task<IEnumerable<CategoriaDto>> GetByUsuarioIdAsync(Guid usuarioId);
        Task<CategoriaDto> CreateAsync(CreateCategoriaDto createDto);
        Task<bool> DeleteAsync(Guid id);
    }
}
