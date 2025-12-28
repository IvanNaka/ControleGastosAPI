using ControleGastos.Domain.Entities;
using ControleGastos.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ControleGastos.Infrastructure.Repositories
{
    public class CategoriaRepository : Repository<Categoria>, ICategoriaRepository
    {
        public CategoriaRepository(ControleGastosDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Categoria>> GetByUsuarioIdAsync(Guid usuarioId)
        {
            return await _dbSet
                .Where(c => c.UsuarioId == usuarioId)
                .OrderBy(c => c.Descricao)
                .ToListAsync();
        }
    }
}
