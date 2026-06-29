using EtsyAnalyzer.Core.DTOs;
using EtsyAnalyzer.Core.ValueObjects;

namespace EtsyAnalyzer.Analytics.Services;

public class CompetitionAnalyzer
{
    private const int LowCompetitionThreshold = 50;
    private const int MediumCompetitionThreshold = 200;

    public CompetitionLevel AnalyzeCompetition(int totalListings)
    {
        return totalListings switch
        {
            < LowCompetitionThreshold => CompetitionLevel.Low,
            < MediumCompetitionThreshold => CompetitionLevel.Medium,
            _ => CompetitionLevel.High
        };
    }

    public List<ShopCompetitorDto> GetTopCompetitors(IEnumerable<ListingDto> listings, int topCount = 10)
    {
        var shopGroups = listings
            .GroupBy(l => l.ShopId)
            .Select(g => new ShopCompetitorDto
            {
                ShopId = g.Key,
                ShopName = g.First().ShopName,
                Url = $"https://www.etsy.com/shop/{g.First().ShopName}",
                ListingsInSearch = g.Count(),
                Rating = g.Any(l => l.Rating.HasValue) ? g.Where(l => l.Rating.HasValue).Average(l => l.Rating!.Value) : null,
                ReviewCount = g.Sum(l => l.ReviewCount),
                ListingCount = 0 // Будет заполнено позже из shop details
            })
            .OrderByDescending(s => s.ListingsInSearch)
            .ThenByDescending(s => s.ReviewCount)
            .Take(topCount)
            .ToList();

        return shopGroups;
    }
}
