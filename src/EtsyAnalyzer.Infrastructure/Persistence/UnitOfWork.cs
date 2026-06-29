using EtsyAnalyzer.Core.Interfaces;
using EtsyAnalyzer.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace EtsyAnalyzer.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly EtsyAnalyzerDbContext _context;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(EtsyAnalyzerDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));

        Listings = new ListingRepository(_context);
        Shops = new ShopRepository(_context);
        SearchQueries = new SearchQueryRepository(_context);
    }

    public IListingRepository Listings { get; }
    public IShopRepository Shops { get; }
    public ISearchQueryRepository SearchQueries { get; }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction == null)
            throw new InvalidOperationException("No active transaction to commit");

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            await _transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            _transaction.Dispose();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            _transaction.Dispose();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
