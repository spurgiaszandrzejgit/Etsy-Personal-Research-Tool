using EtsyAnalyzer.Core.DTOs;
using MathNet.Numerics.Statistics;

namespace EtsyAnalyzer.Analytics.Services;

public class PriceAnalyzer
{
    public PriceStatisticsDto AnalyzePrices(IEnumerable<ListingDto> listings)
    {
        var listingsList = listings.ToList();

        if (!listingsList.Any())
        {
            return new PriceStatisticsDto();
        }

        var prices = listingsList.Select(l => (double)l.Price).ToList();
        var currencyCode = listingsList.First().CurrencyCode;

        var statistics = new PriceStatisticsDto
        {
            Average = (decimal)prices.Mean(),
            Median = (decimal)prices.Median(),
            Min = (decimal)prices.Min(),
            Max = (decimal)prices.Max(),
            CurrencyCode = currencyCode,
            PriceRanges = CalculatePriceRanges(listingsList)
        };

        return statistics;
    }

    private List<PriceRangeDto> CalculatePriceRanges(List<ListingDto> listings)
    {
        if (!listings.Any())
            return new List<PriceRangeDto>();

        var minPrice = listings.Min(l => l.Price);
        var maxPrice = listings.Max(l => l.Price);
        var range = maxPrice - minPrice;

        // Создаём 5 диапазонов цен
        var rangeSize = range / 5;
        var ranges = new List<PriceRangeDto>();

        for (int i = 0; i < 5; i++)
        {
            var rangeMin = minPrice + (i * rangeSize);
            var rangeMax = i == 4 ? maxPrice : rangeMin + rangeSize;

            var count = listings.Count(l => l.Price >= rangeMin && l.Price <= rangeMax);
            var percentage = (decimal)count / listings.Count * 100;

            ranges.Add(new PriceRangeDto
            {
                MinPrice = Math.Round(rangeMin, 2),
                MaxPrice = Math.Round(rangeMax, 2),
                Count = count,
                Percentage = Math.Round(percentage, 1)
            });
        }

        return ranges.OrderBy(r => r.MinPrice).ToList();
    }
}
