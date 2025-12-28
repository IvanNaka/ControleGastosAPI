using ControleGastos.Domain.Entities;

namespace ControleGastos.Domain.Interfaces.Repositories
{
    public interface IUsuarioRepository : IRepository<Usuario>
    {
        Task<Usuario?> GetByAzureAdIdAsync(string azureAdId);
    }
}
