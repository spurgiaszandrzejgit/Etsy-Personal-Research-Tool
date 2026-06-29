using EtsyAnalyzer.Core.DTOs;
using EtsyAnalyzer.EtsyApi.Models;

namespace EtsyAnalyzer.EtsyApi.Mappers;

public static class ShopMapper
{
    public static ShopDto ToDto(this EtsyShopResponse response)
    {
        return new ShopDto
        {
            ShopId = response.ShopId,
            ShopName = response.ShopName,
            Url = response.Url,
            ListingCount = response.ListingActiveCount + response.DigitalListingCount,
            Rating = null, // Etsy API v3 не предоставляет рейтинг магазина напрямую
            ReviewCount = 0
        };
    }
}
