namespace c_Tasking.Utilities;

using c_Tasking.Core;

/// <summary>
/// Utility for retrying tasks with configurable retry policies and exponential backoff.
/// Supports both sync and async operations with optional custom retry predicates.
/// </summary>
public static class TaskRetry
{
    // ===== Async Methods with Return Value =====

    /// <summary>
    /// Retries a task up to maxAttempts times with exponential backoff.
    /// </summary>
    /// <param name="task">The async task function to execute.</param>
    /// <param name="maxAttempts">Maximum retry attempts (default: 3).</param>
    /// <param name="initialDelayMilliseconds">Initial delay in milliseconds for exponential backoff (default: 100).</param>
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
    /// <param name="initialDelayMilliseconds">Initial delay in milliseconds for exponential backoff.</param>
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
                ErrorHandler.Instance.Log(ex, $"TaskRetry.ExecuteWithRetry<T> (attempt {attempt}/{maxAttempts})");
                var delayMilliseconds = initialDelayMilliseconds * (int)Math.Pow(2, attempt - 1);
                await Task.Delay(delayMilliseconds);
            }
        }

        // Final attempt without catching
        return await task();
    }

    // ===== Async Methods without Return Value =====

    /// <summary>
    /// Retries a task (without return value) up to maxAttempts times with exponential backoff.
    /// </summary>
    /// <param name="task">The async task to run and possibly retry.</param>
    /// <param name="maxAttempts">Maximum retry attempts (default: 3).</param>
    /// <param name="initialDelayMilliseconds">Initial delay in milliseconds for exponential backoff (default: 100).</param>
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
    /// <param name="initialDelayMilliseconds">Initial delay in milliseconds for exponential backoff.</param>
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
                ErrorHandler.Instance.Log(ex, $"TaskRetry.ExecuteWithRetry (attempt {attempt}/{maxAttempts})");
                var delayMilliseconds = initialDelayMilliseconds * (int)Math.Pow(2, attempt - 1);
                await Task.Delay(delayMilliseconds);
            }
        }

        // Final attempt without catching
        await task();
    }

    // ===== Sync Methods with Return Value =====

    /// <summary>
    /// Retries a synchronous task up to maxAttempts times with exponential backoff.
    /// </summary>
    /// <param name="task">The synchronous function to run.</param>
    /// <param name="maxAttempts">Maximum retry attempts (default: 3).</param>
    /// <param name="initialDelayMilliseconds">Initial delay in milliseconds for exponential backoff (default: 100).</param>
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
    /// <param name="initialDelayMilliseconds">Initial delay in milliseconds for exponential backoff.</param>
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
                ErrorHandler.Instance.Log(ex, $"TaskRetry.Execute<T> (attempt {attempt}/{maxAttempts})");
                var delayMilliseconds = initialDelayMilliseconds * (int)Math.Pow(2, attempt - 1);
                Thread.Sleep(delayMilliseconds);
            }
        }

        // Final attempt without catching
        return task();
    }

    // ===== Sync Methods without Return Value =====

    /// <summary>
    /// Retries a synchronous action up to maxAttempts times with exponential backoff.
    /// </summary>
    /// <param name="task">The synchronous action to run.</param>
    /// <param name="maxAttempts">Maximum retry attempts (default: 3).</param>
    /// <param name="initialDelayMilliseconds">Initial delay in milliseconds for exponential backoff (default: 100).</param>
    public static void Execute(
        Action task,
        int maxAttempts = 3,
        int initialDelayMilliseconds = 100)
    {
        Execute(task, maxAttempts, initialDelayMilliseconds, null);
    }

    /// <summary>
    /// Retries a synchronous action up to maxAttempts times with exponential backoff and custom exception filter.
    /// </summary>
    /// <param name="task">The synchronous action to run.</param>
    /// <param name="maxAttempts">Maximum retry attempts.</param>
    /// <param name="initialDelayMilliseconds">Initial delay in milliseconds for exponential backoff.</param>
    /// <param name="shouldRetry">Optional predicate to decide whether an exception should trigger retry.</param>
    public static void Execute(
        Action task,
        int maxAttempts,
        int initialDelayMilliseconds,
        Func<Exception, bool>? shouldRetry)
    {
        shouldRetry ??= _ => true;

        for (int attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                task();
                return;
            }
            catch (Exception ex) when (attempt < maxAttempts && shouldRetry(ex))
            {
                ErrorHandler.Instance.Log(ex, $"TaskRetry.Execute (attempt {attempt}/{maxAttempts})");
                var delayMilliseconds = initialDelayMilliseconds * (int)Math.Pow(2, attempt - 1);
                Thread.Sleep(delayMilliseconds);
            }
        }

        // Final attempt without catching
        task();
    } 
    }
