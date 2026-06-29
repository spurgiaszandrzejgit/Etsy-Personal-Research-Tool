namespace EtsyAnalyzer.Core.Interfaces;

/// <summary>
/// Unit of Work паттерн для управления транзакциями и координации репозиториев
/// </summary>
public interface IUnitOfWork : IDisposable
{
    IListingRepository Listings { get; }
    IShopRepository Shops { get; }
    ISearchQueryRepository SearchQueries { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
