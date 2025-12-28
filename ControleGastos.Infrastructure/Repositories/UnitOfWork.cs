using ControleGastos.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace ControleGastos.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ControleGastosDbContext _context;
        private IDbContextTransaction? _transaction;

        public ICategoriaRepository Categorias { get; }
        public IPessoaRepository Pessoas { get; }
        public IUsuarioRepository Usuarios { get; }
        public ITransacaoRepository Transacoes { get; }

        public UnitOfWork(
            ControleGastosDbContext context,
            ICategoriaRepository categoriaRepository,
            IPessoaRepository pessoaRepository,
            IUsuarioRepository usuarioRepository,
            ITransacaoRepository transacaoRepository)
        {
            _context = context;
            Categorias = categoriaRepository;
            Pessoas = pessoaRepository;
            Usuarios = usuarioRepository;
            Transacoes = transacaoRepository;
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                if (_transaction != null)
                {
                    await _transaction.CommitAsync();
                }
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}
