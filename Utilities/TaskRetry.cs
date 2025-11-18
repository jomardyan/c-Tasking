namespace c_Tasking.Utilities;

using c_Tasking.Core;

/// <summary>
/// Utility for retrying tasks with configurable retry policies.
/// </summary>
public static class TaskRetry
{
    /// <summary>
    /// Retries a task up to maxAttempts times with exponential backoff.
    /// </summary>
    /// <param name="task">The async task function to execute.</param>
    /// <param name="maxAttempts">Maximum retry attempts.</param>
    /// <param name="initialDelayMilliseconds">Initial delay in milliseconds used for exponential backoff.</param>
    /// <returns>The result of the task when it succeeds.</returns>
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
    /// <param name="task">The async task function to execute.</param>
    /// <param name="maxAttempts">Maximum retry attempts.</param>
    /// <param name="initialDelayMilliseconds">Initial delay in milliseconds used for exponential backoff.</param>
    /// <param name="shouldRetry">Optional predicate to decide whether an exception should trigger a retry.</param>
    /// <returns>The result when the operation succeeds.</returns>
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
                ErrorHandler.Instance.Log(ex, "TaskRetry.ExecuteWithRetry<T>");
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
    /// <param name="task">The async task to run and possibly retry.</param>
    /// <param name="maxAttempts">Maximum retry attempts.</param>
    /// <param name="initialDelayMilliseconds">Initial delay in milliseconds used for exponential backoff.</param>
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
    /// <param name="task">The async task to run and possibly retry.</param>
    /// <param name="maxAttempts">Maximum retry attempts.</param>
    /// <param name="initialDelayMilliseconds">Initial delay in milliseconds used for exponential backoff.</param>
    /// <param name="shouldRetry">Optional predicate to decide whether an exception should trigger a retry.</param>
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
                ErrorHandler.Instance.Log(ex, "TaskRetry.ExecuteWithRetry (non-generic)");
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
    /// <param name="task">The synchronous function to run.</param>
    /// <param name="maxAttempts">Maximum retry attempts.</param>
    /// <param name="initialDelayMilliseconds">Initial delay in milliseconds used for exponential backoff.</param>
    /// <returns>The result from the function.</returns>
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
    /// <param name="task">The synchronous function to run.</param>
    /// <param name="maxAttempts">Maximum retry attempts.</param>
    /// <param name="initialDelayMilliseconds">Initial delay in milliseconds used for exponential backoff.</param>
    /// <param name="shouldRetry">Optional predicate to decide whether an exception should trigger retry.</param>
    /// <returns>The result from the function.</returns>
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
                ErrorHandler.Instance.Log(ex, "TaskRetry.Execute<T>");
                var delayMilliseconds = initialDelayMilliseconds * (int)Math.Pow(2, attempt - 1);
                Thread.Sleep(delayMilliseconds);
            }
        }

        // Final attempt without catching
        return task();
    }
}
