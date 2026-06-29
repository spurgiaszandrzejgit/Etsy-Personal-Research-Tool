using EtsyAnalyzer.Core.Entities;
using EtsyAnalyzer.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EtsyAnalyzer.Infrastructure.Persistence.Repositories;

public class ShopRepository : IShopRepository
{
    private readonly EtsyAnalyzerDbContext _context;

    public ShopRepository(EtsyAnalyzerDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Shop?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _context.Shops
            .Include(s => s.Listings)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<Shop?> GetByShopIdAsync(long shopId, CancellationToken cancellationToken = default)
    {
        return await _context.Shops
            .Include(s => s.Listings)
            .FirstOrDefaultAsync(s => s.ShopId == shopId, cancellationToken);
    }

    public async Task<IEnumerable<Shop>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Shops
            .Include(s => s.Listings)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Shop shop, CancellationToken cancellationToken = default)
    {
        await _context.Shops.AddAsync(shop, cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<Shop> shops, CancellationToken cancellationToken = default)
    {
        await _context.Shops.AddRangeAsync(shops, cancellationToken);
    }

    public Task UpdateAsync(Shop shop, CancellationToken cancellationToken = default)
    {
        _context.Shops.Update(shop);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var shop = await GetByIdAsync(id, cancellationToken);
        if (shop != null)
        {
            _context.Shops.Remove(shop);
        }
    }
}
