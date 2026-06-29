namespace EtsyAnalyzer.Core.Exceptions;

/// <summary>
/// Базовое исключение для всех ошибок приложения
/// </summary>
public class EtsyAnalyzerException : Exception
{
    public EtsyAnalyzerException() : base()
    {
    }

    public EtsyAnalyzerException(string message) : base(message)
    {
    }

    public EtsyAnalyzerException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
}
