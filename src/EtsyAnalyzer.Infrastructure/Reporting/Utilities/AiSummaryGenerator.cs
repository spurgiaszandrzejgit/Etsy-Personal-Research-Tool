using EtsyAnalyzer.Core.DTOs;
using EtsyAnalyzer.Core.ValueObjects;
using System.Text;

namespace EtsyAnalyzer.Infrastructure.Reporting.Utilities;

/// <summary>
/// Генератор текстового AI-подобного резюме на основе метрик
/// </summary>
public static class AiSummaryGenerator
{
    /// <summary>
    /// Сгенерировать текстовое резюме для ниши
    /// </summary>
    public static string GenerateSummary(AnalyticsSummaryDto summary)
    {
        var sb = new StringBuilder();

        // Заголовок
        sb.AppendLine($"📊 ANALYSIS SUMMARY: \"{summary.SearchQuery}\"");
        sb.AppendLine();

        // Общий вердикт на основе оценки
        sb.AppendLine(GetOverallVerdict((double)summary.NicheScore.Value));
        sb.AppendLine();

        // Анализ конкуренции
        sb.AppendLine("🏪 COMPETITION ANALYSIS:");
        sb.AppendLine(GetCompetitionAnalysis(summary));
        sb.AppendLine();

        // Анализ цен
        sb.AppendLine("💰 PRICING ANALYSIS:");
        sb.AppendLine(GetPricingAnalysis(summary));
        sb.AppendLine();

        // Рекомендации
        sb.AppendLine("💡 RECOMMENDATIONS:");
        sb.AppendLine(GetRecommendations(summary));
        sb.AppendLine();

        // Итоговая рекомендация
        sb.AppendLine("🎯 DECISION:");
        sb.AppendLine(GetFinalDecision((double)summary.NicheScore.Value));

        return sb.ToString();
    }

    private static string GetOverallVerdict(double nicheScore)
    {
        if (nicheScore >= 8.0)
            return "✅ Excellent opportunity! This niche shows strong potential for new sellers.";
        else if (nicheScore >= 6.5)
            return "✅ Good opportunity. This niche has reasonable potential with manageable competition.";
        else if (nicheScore >= 5.0)
            return "⚠️ Moderate opportunity. Success will require differentiation and quality products.";
        else if (nicheScore >= 3.5)
            return "⚠️ Challenging niche. High competition or unfavorable pricing may limit success.";
        else
            return "❌ Not recommended. This niche shows poor indicators for new sellers.";
    }

    private static string GetCompetitionAnalysis(AnalyticsSummaryDto summary)
    {
        var sb = new StringBuilder();
        var competition = summary.CompetitionLevel;

        if (competition == CompetitionLevel.Low)
        {
            sb.AppendLine($"  • Competition Level: LOW ({summary.UniqueShops} unique shops)");
            sb.AppendLine("  • Low shop density indicates room for new sellers.");
            sb.AppendLine("  • Easier to gain visibility and establish presence.");
        }
        else if (competition == CompetitionLevel.Medium)
        {
            sb.AppendLine($"  • Competition Level: MEDIUM ({summary.UniqueShops} unique shops)");
            sb.AppendLine("  • Moderate competition - differentiation is key.");
            sb.AppendLine("  • Quality products and unique value proposition needed.");
        }
        else
        {
            sb.AppendLine($"  • Competition Level: HIGH ({summary.UniqueShops} unique shops)");
            sb.AppendLine("  • Saturated market with many established sellers.");
            sb.AppendLine("  • Requires strong branding and exceptional product quality.");
        }

        // Shop density
        var shopDensity = summary.UniqueShops / (double)summary.TotalListings;
        if (shopDensity > 0.7)
            sb.AppendLine("  • High shop diversity - many sellers offer 1-2 items.");
        else if (shopDensity < 0.3)
            sb.AppendLine("  • Low shop diversity - dominated by few large sellers.");
        else
            sb.AppendLine("  • Balanced shop distribution.");

        return sb.ToString();
    }

    private static string GetPricingAnalysis(AnalyticsSummaryDto summary)
    {
        var sb = new StringBuilder();

        sb.AppendLine($"  • Average Price: {summary.CurrencySymbol}{summary.AveragePrice:F2}");
        sb.AppendLine($"  • Median Price: {summary.CurrencySymbol}{summary.MedianPrice:F2}");
        sb.AppendLine($"  • Price Range: {summary.CurrencySymbol}{summary.MinPrice:F2} - {summary.CurrencySymbol}{summary.MaxPrice:F2}");

        // Standard deviation analysis
        var stdDev = summary.StandardDeviation ?? 0;
        var avgPrice = summary.AveragePrice;
        var coefficientOfVariation = avgPrice > 0 ? (stdDev / avgPrice) * 100 : 0;

        if (coefficientOfVariation > 50)
        {
            sb.AppendLine("  • Wide price spread - diverse product quality/features.");
            sb.AppendLine("  • Opportunity for both budget and premium positioning.");
        }
        else if (coefficientOfVariation < 20)
        {
            sb.AppendLine("  • Narrow price spread - commoditized market.");
            sb.AppendLine("  • Price competition likely; focus on quality and branding.");
        }
        else
        {
            sb.AppendLine("  • Moderate price spread - room for differentiation.");
        }

        // Average vs Median comparison
        if (summary.AveragePrice > summary.MedianPrice * 1.2m)
        {
            sb.AppendLine("  • Average exceeds median - some high-priced outliers.");
            sb.AppendLine("  • Premium segment exists but most products are mid-priced.");
        }
        else if (summary.MedianPrice > summary.AveragePrice * 1.2m)
        {
            sb.AppendLine("  • Median exceeds average - some low-priced outliers.");
            sb.AppendLine("  • Budget segment exists but most products are higher-priced.");
        }

        return sb.ToString();
    }

    private static string GetRecommendations(AnalyticsSummaryDto summary)
    {
        var sb = new StringBuilder();
        var recommendations = new List<string>();

        // Competition-based recommendations
        if (summary.CompetitionLevel == CompetitionLevel.Low)
        {
            recommendations.Add("✓ Enter quickly to establish market presence.");
            recommendations.Add("✓ Focus on SEO and keyword optimization for visibility.");
        }
        else if (summary.CompetitionLevel == CompetitionLevel.High)
        {
            recommendations.Add("⚠ Find a unique sub-niche or angle.");
            recommendations.Add("⚠ Invest in professional photography and branding.");
            recommendations.Add("⚠ Consider premium positioning to stand out.");
        }

        // Price-based recommendations
        if (summary.AveragePrice < 20m)
        {
            recommendations.Add("⚠ Low average price - ensure profit margins are viable.");
            recommendations.Add("✓ Volume sales strategy may be needed.");
        }
        else if (summary.AveragePrice > 100m)
        {
            recommendations.Add("✓ High price point - focus on quality and exclusivity.");
            recommendations.Add("⚠ Lower sales volume expected - ensure conversion rate.");
        }

        // Niche score recommendations
        var nicheValue = (double)summary.NicheScore.Value;
        if (nicheValue >= 7.0)
        {
            recommendations.Add("✓ Strong niche - start with 5-10 product variations.");
            recommendations.Add("✓ Build brand around this category.");
        }
        else if (nicheValue < 5.0)
        {
            recommendations.Add("⚠ Consider testing with 1-2 products before scaling.");
            recommendations.Add("⚠ Have backup niches ready.");
        }

        // Top keywords insight
        if (summary.TopKeywords.Any())
        {
            var topKeyword = summary.TopKeywords.First();
            recommendations.Add($"✓ Focus on keyword: \"{topKeyword.Keyword}\" (used {topKeyword.Frequency}x).");
        }

        foreach (var rec in recommendations)
        {
            sb.AppendLine($"  {rec}");
        }

        return sb.ToString();
    }

    private static string GetFinalDecision(double nicheScore)
    {
        if (nicheScore >= 8.0)
            return "  ✅ RECOMMENDED - Strong opportunity for new sellers. Proceed with confidence.";
        else if (nicheScore >= 6.5)
            return "  ✅ RECOMMENDED - Good potential. Research top competitors before launching.";
        else if (nicheScore >= 5.0)
            return "  ⚠️ PROCEED WITH CAUTION - Test with small inventory first.";
        else if (nicheScore >= 3.5)
            return "  ⚠️ NOT IDEAL - Consider alternative niches unless you have unique advantages.";
        else
            return "  ❌ NOT RECOMMENDED - High risk. Explore other opportunities.";
    }
}
