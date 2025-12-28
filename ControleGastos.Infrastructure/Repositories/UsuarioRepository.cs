using ControleGastos.Domain.Entities;
using ControleGastos.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ControleGastos.Infrastructure.Repositories
{
    public class UsuarioRepository : Repository<Usuario>, IUsuarioRepository
    {
        public UsuarioRepository(ControleGastosDbContext context) : base(context)
        {
        }

        public async Task<Usuario?> GetByAzureAdIdAsync(string azureAdId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(u => u.AzureAdId == azureAdId);
        }
    }
}
