namespace c_Tasking.Utilities;

/// <summary>
/// Utility for retrying tasks with configurable retry policies.
/// </summary>
public static class TaskRetry
{
    /// <summary>
    /// Retries a task up to maxAttempts times with exponential backoff.
    /// </summary>
    public static async Task<T> ExecuteWithRetry<T>(
        Func<Task<T>> task,
        int maxAttempts = 3,
        int initialDelayMilliseconds = 100)
    {
        return await ExecuteWithRetry(task, maxAttempts, initialDelayMilliseconds, null);
    }

    /// <summary>
    /// Retries a task up to maxAttempts times with exponential backoff and custom exception filter.
    /// </summary>
    public static async Task<T> ExecuteWithRetry<T>(
        Func<Task<T>> task,
        int maxAttempts,
        int initialDelayMilliseconds,
        Func<Exception, bool>? shouldRetry)
    {
        shouldRetry ??= _ => true;

        for (int attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                return await task();
            }
            catch (Exception ex) when (attempt < maxAttempts && shouldRetry(ex))
            {
                var delayMilliseconds = initialDelayMilliseconds * (int)Math.Pow(2, attempt - 1);
                await Task.Delay(delayMilliseconds);
            }
        }

        // Final attempt without catching
        return await task();
    }

    /// <summary>
    /// Retries a task (without return value) up to maxAttempts times with exponential backoff.
    /// </summary>
    public static async Task ExecuteWithRetry(
        Func<Task> task,
        int maxAttempts = 3,
        int initialDelayMilliseconds = 100)
    {
        await ExecuteWithRetry(task, maxAttempts, initialDelayMilliseconds, null);
    }

    /// <summary>
    /// Retries a task (without return value) up to maxAttempts times with exponential backoff and custom exception filter.
    /// </summary>
    public static async Task ExecuteWithRetry(
        Func<Task> task,
        int maxAttempts,
        int initialDelayMilliseconds,
        Func<Exception, bool>? shouldRetry)
    {
        shouldRetry ??= _ => true;

        for (int attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                await task();
                return;
            }
            catch (Exception ex) when (attempt < maxAttempts && shouldRetry(ex))
            {
                var delayMilliseconds = initialDelayMilliseconds * (int)Math.Pow(2, attempt - 1);
                await Task.Delay(delayMilliseconds);
            }
        }

        // Final attempt without catching
        await task();
    }

    /// <summary>
    /// Retries a synchronous task up to maxAttempts times with exponential backoff.
    /// </summary>
    public static T Execute<T>(
        Func<T> task,
        int maxAttempts = 3,
        int initialDelayMilliseconds = 100)
    {
        return Execute(task, maxAttempts, initialDelayMilliseconds, null);
    }

    /// <summary>
    /// Retries a synchronous task up to maxAttempts times with exponential backoff and custom exception filter.
    /// </summary>
    public static T Execute<T>(
        Func<T> task,
        int maxAttempts,
        int initialDelayMilliseconds,
        Func<Exception, bool>? shouldRetry)
    {
        shouldRetry ??= _ => true;

        for (int attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                return task();
            }
            catch (Exception ex) when (attempt < maxAttempts && shouldRetry(ex))
            {
                var delayMilliseconds = initialDelayMilliseconds * (int)Math.Pow(2, attempt - 1);
                Thread.Sleep(delayMilliseconds);
            }
        }

        // Final attempt without catching
        return task();
    }
}
