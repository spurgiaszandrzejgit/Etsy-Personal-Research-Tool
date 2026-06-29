namespace EtsyAnalyzer.Core.DTOs;

public class PriceStatisticsDto
{
    public decimal Average { get; set; }
    public decimal Median { get; set; }
    public decimal Min { get; set; }
    public decimal Max { get; set; }
    public string CurrencyCode { get; set; } = "USD";
    public List<PriceRangeDto> PriceRanges { get; set; } = new();
}

public class PriceRangeDto
{
    public decimal MinPrice { get; set; }
    public decimal MaxPrice { get; set; }
    public int Count { get; set; }
    public decimal Percentage { get; set; }

    public string Label => $"{MinPrice:F0}-{MaxPrice:F0}";
}
