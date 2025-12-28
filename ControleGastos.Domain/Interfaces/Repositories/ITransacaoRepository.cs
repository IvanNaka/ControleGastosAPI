using ControleGastos.Domain.Entities;
using ControleGastos.Domain.Enums;

namespace ControleGastos.Domain.Interfaces.Repositories
{
    public interface ITransacaoRepository : IRepository<Transacao>
    {
        Task<IEnumerable<Transacao>> GetByUsuarioIdAsync(Guid usuarioId);
        Task<IEnumerable<Transacao>> GetByPessoaIdAsync(Guid pessoaId);
        Task<Transacao?> GetByIdWithDetailsAsync(Guid id);
    }
}
