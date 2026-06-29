namespace EtsyAnalyzer.Core.ValueObjects;

public record Money
{
    public decimal Amount { get; init; }
    public string CurrencyCode { get; init; } = "USD";

    public Money(decimal amount, string currencyCode = "USD")
    {
        if (amount < 0)
            throw new ArgumentException("Amount cannot be negative", nameof(amount));

        Amount = amount;
        CurrencyCode = currencyCode.ToUpperInvariant();
    }

    public override string ToString() => $"{Amount:F2} {CurrencyCode}";
}
