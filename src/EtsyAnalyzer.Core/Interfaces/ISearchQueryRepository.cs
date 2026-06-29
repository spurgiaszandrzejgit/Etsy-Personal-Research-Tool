using EtsyAnalyzer.Core.Entities;

namespace EtsyAnalyzer.Core.Interfaces;

public interface ISearchQueryRepository
{
    Task<SearchQuery?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<IEnumerable<SearchQuery>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<SearchQuery?> GetByQueryStringAsync(string query, CancellationToken cancellationToken = default);
    Task AddAsync(SearchQuery searchQuery, CancellationToken cancellationToken = default);
    Task UpdateAsync(SearchQuery searchQuery, CancellationToken cancellationToken = default);
    Task DeleteAsync(long id, CancellationToken cancellationToken = default);
}
