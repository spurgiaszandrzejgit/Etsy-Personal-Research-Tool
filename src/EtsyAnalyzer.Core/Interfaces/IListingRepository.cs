using EtsyAnalyzer.Core.Entities;

namespace EtsyAnalyzer.Core.Interfaces;

public interface IListingRepository
{
    Task<Listing?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<Listing?> GetByListingIdAsync(long listingId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Listing>> GetBySearchQueryIdAsync(long searchQueryId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Listing>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Listing listing, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<Listing> listings, CancellationToken cancellationToken = default);
    Task UpdateAsync(Listing listing, CancellationToken cancellationToken = default);
    Task DeleteAsync(long id, CancellationToken cancellationToken = default);
}
