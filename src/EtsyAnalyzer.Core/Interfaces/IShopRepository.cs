using EtsyAnalyzer.Core.Entities;

namespace EtsyAnalyzer.Core.Interfaces;

public interface IShopRepository
{
    Task<Shop?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<Shop?> GetByShopIdAsync(long shopId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Shop>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Shop shop, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<Shop> shops, CancellationToken cancellationToken = default);
    Task UpdateAsync(Shop shop, CancellationToken cancellationToken = default);
    Task DeleteAsync(long id, CancellationToken cancellationToken = default);
}
