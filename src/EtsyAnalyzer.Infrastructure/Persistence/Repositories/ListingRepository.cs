using EtsyAnalyzer.Core.Entities;
using EtsyAnalyzer.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EtsyAnalyzer.Infrastructure.Persistence.Repositories;

public class ListingRepository : IListingRepository
{
    private readonly EtsyAnalyzerDbContext _context;

    public ListingRepository(EtsyAnalyzerDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Listing?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _context.Listings
            .Include(l => l.Shop)
            .Include(l => l.SearchQuery)
            .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
    }

    public async Task<Listing?> GetByListingIdAsync(long listingId, CancellationToken cancellationToken = default)
    {
        return await _context.Listings
            .Include(l => l.Shop)
            .Include(l => l.SearchQuery)
            .FirstOrDefaultAsync(l => l.ListingId == listingId, cancellationToken);
    }

    public async Task<IEnumerable<Listing>> GetBySearchQueryIdAsync(long searchQueryId, CancellationToken cancellationToken = default)
    {
        return await _context.Listings
            .Include(l => l.Shop)
            .Where(l => l.SearchQueryId == searchQueryId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Listing>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Listings
            .Include(l => l.Shop)
            .Include(l => l.SearchQuery)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Listing listing, CancellationToken cancellationToken = default)
    {
        await _context.Listings.AddAsync(listing, cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<Listing> listings, CancellationToken cancellationToken = default)
    {
        await _context.Listings.AddRangeAsync(listings, cancellationToken);
    }

    public Task UpdateAsync(Listing listing, CancellationToken cancellationToken = default)
    {
        _context.Listings.Update(listing);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var listing = await GetByIdAsync(id, cancellationToken);
        if (listing != null)
        {
            _context.Listings.Remove(listing);
        }
    }
}
