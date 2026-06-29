using EtsyAnalyzer.Core.Entities;
using EtsyAnalyzer.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EtsyAnalyzer.Infrastructure.Persistence.Repositories;

public class SearchQueryRepository : ISearchQueryRepository
{
    private readonly EtsyAnalyzerDbContext _context;

    public SearchQueryRepository(EtsyAnalyzerDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<SearchQuery?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _context.SearchQueries
            .Include(sq => sq.Listings)
            .FirstOrDefaultAsync(sq => sq.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<SearchQuery>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SearchQueries
            .Include(sq => sq.Listings)
            .ToListAsync(cancellationToken);
    }

    public async Task<SearchQuery?> GetByQueryStringAsync(string query, CancellationToken cancellationToken = default)
    {
        return await _context.SearchQueries
            .Include(sq => sq.Listings)
            .FirstOrDefaultAsync(sq => sq.Query == query, cancellationToken);
    }

    public async Task AddAsync(SearchQuery searchQuery, CancellationToken cancellationToken = default)
    {
        await _context.SearchQueries.AddAsync(searchQuery, cancellationToken);
    }

    public Task UpdateAsync(SearchQuery searchQuery, CancellationToken cancellationToken = default)
    {
        _context.SearchQueries.Update(searchQuery);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var searchQuery = await GetByIdAsync(id, cancellationToken);
        if (searchQuery != null)
        {
            _context.SearchQueries.Remove(searchQuery);
        }
    }
}
