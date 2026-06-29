using EtsyAnalyzer.Core.DTOs;
using EtsyAnalyzer.Core.Interfaces;
using EtsyAnalyzer.EtsyApi.Client;
using EtsyAnalyzer.EtsyApi.Mappers;

namespace EtsyAnalyzer.EtsyApi.Services;

public class EtsyDataSource : IDataSource
{
    private readonly EtsyApiClient _apiClient;
    private readonly Dictionary<long, string> _shopNameCache = new();

    public EtsyDataSource(EtsyApiClient apiClient)
    {
        _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
    }

    public string SourceName => "Etsy API v3";

    public async Task<IEnumerable<ListingDto>> SearchListingsAsync(
        string query, 
        int limit = 100, 
        CancellationToken cancellationToken = default)
    {
        var response = await _apiClient.SearchListingsAsync(query, limit, cancellationToken);

        var listings = new List<ListingDto>();

        foreach (var listing in response.Results)
        {
            // Получаем имя магазина (кэшируем для производительности)
            var shopName = await GetShopNameAsync(listing.ShopId, cancellationToken);

            var dto = listing.ToDto(shopName);
            listings.Add(dto);
        }

        return listings;
    }

    public async Task<ShopDto?> GetShopDetailsAsync(
        long shopId, 
        CancellationToken cancellationToken = default)
    {
        var response = await _apiClient.GetShopByIdAsync(shopId, cancellationToken);
        return response?.ToDto();
    }

    private async Task<string> GetShopNameAsync(long shopId, CancellationToken cancellationToken)
    {
        if (_shopNameCache.TryGetValue(shopId, out var cachedName))
        {
            return cachedName;
        }

        var shop = await GetShopDetailsAsync(shopId, cancellationToken);
        var shopName = shop?.ShopName ?? $"Shop_{shopId}";

        _shopNameCache[shopId] = shopName;

        return shopName;
    }
}
