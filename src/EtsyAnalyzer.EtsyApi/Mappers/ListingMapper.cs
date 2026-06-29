using EtsyAnalyzer.Core.DTOs;
using EtsyAnalyzer.EtsyApi.Models;

namespace EtsyAnalyzer.EtsyApi.Mappers;

public static class ListingMapper
{
    public static ListingDto ToDto(this EtsyListingResponse response, string shopName = "")
    {
        return new ListingDto
        {
            ListingId = response.ListingId,
            Title = response.Title,
            Price = response.Price?.GetDecimalPrice() ?? 0,
            CurrencyCode = response.Price?.CurrencyCode ?? "USD",
            Description = response.Description ?? string.Empty,
            CategoryPath = response.TaxonomyPath != null ? string.Join(" > ", response.TaxonomyPath) : null,
            Tags = response.Tags ?? new List<string>(),
            Url = response.Url,
            ShopId = response.ShopId,
            ShopName = shopName,
            Rating = null, // Etsy API v3 не возвращает рейтинг напрямую
            ReviewCount = 0,
            ImageUrl = response.Images?.FirstOrDefault()?.Url570xN ?? response.Images?.FirstOrDefault()?.UrlFullxFull
        };
    }
}
