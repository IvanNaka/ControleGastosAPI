using ControleGastos.Domain.Entities;

namespace ControleGastos.Domain.Interfaces.Repositories
{
    public interface ICategoriaRepository : IRepository<Categoria>
    {
        Task<IEnumerable<Categoria>> GetByUsuarioIdAsync(Guid usuarioId);
    }
}
