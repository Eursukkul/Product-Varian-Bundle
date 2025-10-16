namespace FlowAccount.Domain.Interfaces;

/// <summary>
/// Unit of Work Pattern - จัดการ Transaction และ Coordination ระหว่าง Repositories
/// </summary>
public interface IUnitOfWork : IDisposable
{
    // Repositories
    IProductRepository Products { get; }
    IVariantRepository Variants { get; }
    IBundleRepository Bundles { get; }
    IStockRepository Stocks { get; }

    // Transaction Methods
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    Task BeginTransactionAsync();

    Task CommitTransactionAsync();

    Task RollbackTransactionAsync();
}
