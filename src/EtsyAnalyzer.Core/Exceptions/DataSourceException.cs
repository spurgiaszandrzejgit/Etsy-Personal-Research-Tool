namespace EtsyAnalyzer.Core.Exceptions;

/// <summary>
/// Исключение при работе с источниками данных (API, Web Scraper и т.д.)
/// </summary>
public class DataSourceException : EtsyAnalyzerException
{
    public string? SourceName { get; }

    public DataSourceException() : base()
    {
    }

    public DataSourceException(string message) : base(message)
    {
    }

    public DataSourceException(string message, string sourceName) : base(message)
    {
        SourceName = sourceName;
    }

    public DataSourceException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }

    public DataSourceException(string message, string sourceName, Exception innerException) 
        : base(message, innerException)
    {
        SourceName = sourceName;
    }
}
