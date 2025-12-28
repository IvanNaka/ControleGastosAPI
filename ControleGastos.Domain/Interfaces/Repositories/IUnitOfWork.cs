namespace ControleGastos.Domain.Interfaces.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        ICategoriaRepository Categorias { get; }
        IPessoaRepository Pessoas { get; }
        IUsuarioRepository Usuarios { get; }
        ITransacaoRepository Transacoes { get; }
        
        Task<int> CompleteAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
