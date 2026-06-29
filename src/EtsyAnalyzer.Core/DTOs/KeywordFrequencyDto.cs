namespace EtsyAnalyzer.Core.DTOs;

public class KeywordFrequencyDto
{
    public string Keyword { get; set; } = string.Empty;
    public int Frequency { get; set; }
    public decimal Percentage { get; set; }
}
