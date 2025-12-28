using ControleGastos.Application.DTOs;
using ControleGastos.Domain.Enums;

namespace ControleGastos.Application.Interfaces
{
    public interface ITransacaoService
    {
        Task<IEnumerable<TransacaoDto>> GetByUsuarioIdAsync(Guid usuarioId);
        Task<TransacaoDto> CreateAsync(CreateTransacaoDto createDto);
        Task<bool> DeleteAsync(Guid id);
    }
}
