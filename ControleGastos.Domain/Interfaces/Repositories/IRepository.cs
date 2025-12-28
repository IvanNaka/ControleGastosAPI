using ControleGastos.Domain.Entities;
using System.Linq.Expressions;

namespace ControleGastos.Domain.Interfaces.Repositories
{
    public interface IRepository<TEntity> where TEntity : BaseEntity
    {
        Task<TEntity?> GetByIdAsync(Guid id);
        Task AddAsync(TEntity entity);
        void Update(TEntity entity);
        void UpdateRange(IEnumerable<TEntity> entities);
        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate);
    }
}
