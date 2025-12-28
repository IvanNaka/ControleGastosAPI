using ControleGastos.Domain.Entities;
using ControleGastos.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Threading;

namespace ControleGastos.Infrastructure.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
    {
        protected readonly ControleGastosDbContext _context;
        protected readonly DbSet<TEntity> _dbSet;

        public Repository(ControleGastosDbContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }
        public virtual async Task<TEntity?> GetByIdAsync(Guid id)
        {
            return await _dbSet.Where(x => x.Ativo == true && x.Id == id).FirstAsync();
        }

        public virtual async Task AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public virtual void Update(TEntity entity)
        {
            _dbSet.Update(entity);
        }

        public virtual void UpdateRange(IEnumerable<TEntity> entities)
        {
            _dbSet.UpdateRange(entities);
        }
        public virtual async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }
    }
}
