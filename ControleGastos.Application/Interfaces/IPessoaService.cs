using ControleGastos.Application.DTOs;

namespace ControleGastos.Application.Interfaces
{
    public interface IPessoaService
    {
        Task<IEnumerable<PessoaDto>> GetByUsuarioIdAsync(Guid usuarioId);
        Task<PessoaDto> CreateAsync(CreatePessoaDto createDto);
        Task<bool> DeleteAsync(Guid id);
    }
}
