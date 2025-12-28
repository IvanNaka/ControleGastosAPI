using ControleGastos.Domain.Entities;
using ControleGastos.Domain.Enums;
using ControleGastos.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ControleGastos.Infrastructure.Repositories
{
    public class TransacaoRepository : Repository<Transacao>, ITransacaoRepository
    {
        public TransacaoRepository(ControleGastosDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Transacao>> GetByUsuarioIdAsync(Guid usuarioId)
        {
            return await _dbSet
                .Include(t => t.Pessoa)
                .Include(t => t.Categoria)
                .Where(t => t.UsuarioId == usuarioId)
                .OrderByDescending(t => t.DataCriacao)
                .ToListAsync();
        }

        public async Task<IEnumerable<Transacao>> GetByPessoaIdAsync(Guid pessoaId)
        {
            return await _dbSet
                .Include(t => t.Categoria)
                .Where(t => t.PessoaId == pessoaId)
                .OrderByDescending(t => t.DataCriacao)
                .ToListAsync();
        }

        public async Task<Transacao?> GetByIdWithDetailsAsync(Guid id)
        {
            return await _dbSet
                .Include(t => t.Pessoa)
                .Include(t => t.Categoria)
                .Include(t => t.Usuario)
                .FirstOrDefaultAsync(t => t.Id == id);
        }
    }
}
