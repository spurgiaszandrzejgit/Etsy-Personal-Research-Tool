namespace EtsyAnalyzer.Core.ValueObjects;

public enum CompetitionLevel
{
    Low,
    Medium,
    High
}

public static class CompetitionLevelExtensions
{
    public static string ToDisplayString(this CompetitionLevel level) => level switch
    {
        CompetitionLevel.Low => "Low",
        CompetitionLevel.Medium => "Medium",
        CompetitionLevel.High => "High",
        _ => "Unknown"
    };
}
