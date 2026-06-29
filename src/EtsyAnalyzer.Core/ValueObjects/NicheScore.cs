namespace EtsyAnalyzer.Core.ValueObjects;

public record NicheScore
{
    public decimal Value { get; init; }

    public NicheScore(decimal value)
    {
        if (value < 1 || value > 10)
            throw new ArgumentException("Niche score must be between 1 and 10", nameof(value));

        Value = Math.Round(value, 1);
    }

    public string GetRating() => Value switch
    {
        >= 8.0m => "Excellent",
        >= 6.0m => "Good",
        >= 4.0m => "Fair",
        _ => "Poor"
    };

    public override string ToString() => $"{Value:F1}/10 ({GetRating()})";
}
