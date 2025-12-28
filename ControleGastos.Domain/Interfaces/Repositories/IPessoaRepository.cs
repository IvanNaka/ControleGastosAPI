using ControleGastos.Domain.Entities;

namespace ControleGastos.Domain.Interfaces.Repositories
{
    public interface IPessoaRepository : IRepository<Pessoa>
    {
        Task<IEnumerable<Pessoa>> GetByUsuarioIdAsync(Guid usuarioId);
    }
}
