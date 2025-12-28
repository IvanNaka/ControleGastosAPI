using ControleGastos.Domain.Entities;
using ControleGastos.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ControleGastos.Infrastructure.Repositories
{
    public class PessoaRepository : Repository<Pessoa>, IPessoaRepository
    {
        public PessoaRepository(ControleGastosDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Pessoa>> GetByUsuarioIdAsync(Guid usuarioId)
        {
            return await _dbSet
                .Where(p => p.UsuarioId == usuarioId)
                .OrderBy(p => p.Nome)
                .ToListAsync();
        }
    }
}
