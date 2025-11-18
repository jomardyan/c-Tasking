namespace c_Tasking.Core;

/// <summary>
/// Pluggable logging/error handling interface for the library.
/// </summary>
public interface IErrorHandler
{
    /// <summary>
    /// Logs an exception with an optional context message.
    /// </summary>
    /// <param name="exception">Exception to log (may be null for unspecified errors).</param>
    /// <param name="context">Optional contextual tag about where the error originated.</param>
    void Log(Exception? exception, string? context = null);

    /// <summary>
    /// Log a general informational message. Useful for lifecycle events that are not exceptions.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="context">Optional contextual tag.</param>
    void LogMessage(string? message, string? context = null);
}

/// <summary>
/// Global access point for error handling. Defaults to a simple console logger.
/// </summary>
public static class ErrorHandler
{
    private static IErrorHandler _instance = new ConsoleErrorHandler();

    /// <summary>
    /// Global error handler instance used across the library. You may replace this instance with your own implementation.
    /// </summary>
    public static IErrorHandler Instance
    {
        get => _instance;
        set => _instance = value ?? throw new ArgumentNullException(nameof(value));
    }
}

/// <summary>
/// A simple default error handler that writes to the console's standard error.
/// </summary>
public class ConsoleErrorHandler : IErrorHandler
{
    /// <summary>
    /// Logs the provided exception and optional context to console error.
    /// </summary>
    public void Log(Exception? exception, string? context = null)
    {
        var prefix = string.IsNullOrEmpty(context) ? "[c-Tasking]" : $"[c-Tasking:{context}]";
        if (exception == null)
        {
            Console.Error.WriteLine($"{prefix} Unknown error (null exception)");
            return;
        }
        Console.Error.WriteLine($"{prefix} {exception.GetType().Name}: {exception.Message}");
        Console.Error.WriteLine(exception.StackTrace);
    }

    /// <inheritdoc />
    public void LogMessage(string? message, string? context = null)
    {
        var prefix = string.IsNullOrEmpty(context) ? "[c-Tasking]" : $"[c-Tasking:{context}]";
        Console.Error.WriteLine($"{prefix} {message}");
    }
}
