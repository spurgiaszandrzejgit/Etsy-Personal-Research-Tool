using EtsyAnalyzer.Core.DTOs;
using EtsyAnalyzer.Core.ValueObjects;

namespace EtsyAnalyzer.Analytics.Services;

public class NicheScoreCalculator
{
    /// <summary>
    /// Рассчитывает оценку ниши от 1 до 10 на основе нескольких факторов
    /// </summary>
    public NicheScore CalculateScore(AnalyticsSummaryDto summary)
    {
        var scores = new List<decimal>();

        // 1. Оценка конкуренции (чем ниже, тем лучше)
        var competitionScore = summary.CompetitionLevel switch
        {
            CompetitionLevel.Low => 10m,
            CompetitionLevel.Medium => 6m,
            CompetitionLevel.High => 3m,
            _ => 5m
        };
        scores.Add(competitionScore);

        // 2. Оценка размера рынка (оптимальный диапазон 50-500 товаров)
        var marketSizeScore = summary.TotalListings switch
        {
            < 20 => 3m,           // Слишком маленький рынок
            < 50 => 6m,
            < 200 => 9m,          // Оптимальный размер
            < 500 => 7m,
            < 1000 => 5m,
            _ => 3m               // Слишком большая конкуренция
        };
        scores.Add(marketSizeScore);

        // 3. Оценка ценового разброса (большой разброс = возможности)
        var priceRange = summary.PriceStatistics.Max - summary.PriceStatistics.Min;
        var priceRangeScore = priceRange switch
        {
            < 10m => 4m,          // Узкий диапазон
            < 50m => 7m,
            < 100m => 9m,         // Хороший разброс
            < 200m => 8m,
            _ => 6m               // Очень широкий
        };
        scores.Add(priceRangeScore);

        // 4. Оценка средней цены (выше = потенциально лучше)
        var avgPriceScore = summary.PriceStatistics.Average switch
        {
            < 10m => 4m,
            < 25m => 6m,
            < 50m => 8m,
            < 100m => 9m,
            _ => 7m
        };
        scores.Add(avgPriceScore);

        // 5. Разнообразие продавцов (меньше доминирования = лучше)
        var topShopShare = summary.TopShops.Any() 
            ? (decimal)summary.TopShops.First().ListingsInSearch / summary.TotalListings * 100
            : 0;

        var diversityScore = topShopShare switch
        {
            > 50m => 3m,          // Один продавец доминирует
            > 30m => 5m,
            > 20m => 7m,
            > 10m => 9m,
            _ => 10m              // Много равных конкурентов
        };
        scores.Add(diversityScore);

        // Средневзвешенная оценка
        var finalScore = scores.Average();

        return new NicheScore(Math.Round(finalScore, 1));
    }
}
