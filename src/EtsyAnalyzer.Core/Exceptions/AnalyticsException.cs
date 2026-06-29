namespace EtsyAnalyzer.Core.Exceptions;

/// <summary>
/// Исключение при выполнении аналитики
/// </summary>
public class AnalyticsException : EtsyAnalyzerException
{
    public string? Query { get; }

    public AnalyticsException() : base()
    {
    }

    public AnalyticsException(string message) : base(message)
    {
    }

    public AnalyticsException(string message, string query) : base(message)
    {
        Query = query;
    }

    public AnalyticsException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }

    public AnalyticsException(string message, string query, Exception innerException) 
        : base(message, innerException)
    {
        Query = query;
    }
}
