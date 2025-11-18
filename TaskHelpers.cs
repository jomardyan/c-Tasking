namespace c_Tasking;

using c_Tasking.Core;
using c_Tasking.Utilities;
using c_Tasking.Extensions;

/// <summary>
/// Convenience helpers and shortcuts for common task patterns.
/// Provides factory methods for fire-and-forget, quick retries, and common operations.
/// </summary>
public static class TaskHelpers
{
    // ===== Fire-and-Forget Factory Methods =====

    /// <summary>
    /// Creates and fires a task without waiting, with optional error handling.
    /// </summary>
    /// <param name="work">The work to execute.</param>
    /// <param name="onError">Optional error handler if the task fails.</param>
    public static void FireAsync(Func<Task> work, Action<Exception>? onError = null)
    {
        work().FireAndForget(onError);
    }

    /// <summary>
    /// Creates and fires a generic task without waiting, with optional error handling.
    /// </summary>
    /// <typeparam name="T">Return type.</typeparam>
    /// <param name="work">The work to execute.</param>
    /// <param name="onError">Optional error handler if the task fails.</param>
    public static void FireAsync<T>(Func<Task<T>> work, Action<Exception>? onError = null)
    {
        work().FireAndForget(onError);
    }

    // ===== Quick Retry Factory Methods =====

    /// <summary>
    /// Executes async work with automatic retry on failure (3 attempts, 100ms initial backoff).
    /// </summary>
    /// <param name="work">The async work to execute and retry if needed.</param>
    /// <returns>A task representing the retry operation.</returns>
    public static Task RetryAsync(Func<Task> work)
    {
        return work.RetryOnFailure();
    }

    /// <summary>
    /// Executes async work with automatic retry on failure with custom retry predicate.
    /// </summary>
    /// <param name="work">The async work to execute and retry if needed.</param>
    /// <param name="shouldRetry">Predicate to determine if exception should trigger retry.</param>
    /// <param name="maxAttempts">Maximum attempts (default: 3).</param>
    /// <returns>A task representing the retry operation.</returns>
    public static Task RetryAsync(Func<Task> work, Func<Exception, bool> shouldRetry, int maxAttempts = 3)
    {
        return work.RetryOnFailure(shouldRetry, maxAttempts);
    }

    /// <summary>
    /// Executes async work with result and automatic retry on failure.
    /// </summary>
    /// <typeparam name="T">Return type.</typeparam>
    /// <param name="work">The async work to execute and retry if needed.</param>
    /// <returns>A task producing the result.</returns>
    public static Task<T> RetryAsync<T>(Func<Task<T>> work)
    {
        return TaskRetry.ExecuteWithRetry(work);
    }

    /// <summary>
    /// Executes sync work with automatic retry on failure (3 attempts, 100ms initial backoff).
    /// </summary>
    /// <param name="work">The sync work to execute and retry if needed.</param>
    public static void Retry(Action work)
    {
        TaskRetry.Execute(work);
    }

    /// <summary>
    /// Executes sync work with result and automatic retry on failure.
    /// </summary>
    /// <typeparam name="T">Return type.</typeparam>
    /// <param name="work">The sync work to execute and retry if needed.</param>
    /// <returns>The result.</returns>
    public static T Retry<T>(Func<T> work)
    {
        return TaskRetry.Execute(work);
    }

    // ===== Quick Parallel Execution =====

    /// <summary>
    /// Runs multiple async tasks in parallel and waits for all to complete.
    /// </summary>
    /// <param name="tasks">Tasks to run in parallel.</param>
    /// <returns>A task that completes when all tasks complete.</returns>
    public static Task RunInParallel(params Func<Task>[] tasks)
    {
        var executedTasks = tasks.Select(t => t()).ToArray();
        return Task.WhenAll(executedTasks);
    }

    /// <summary>
    /// Runs multiple async tasks in parallel and collects their results.
    /// </summary>
    /// <typeparam name="T">Return type of tasks.</typeparam>
    /// <param name="tasks">Task factories to run in parallel.</param>
    /// <returns>A task producing an array of results.</returns>
    public static async Task<T[]> RunInParallel<T>(params Func<Task<T>>[] tasks)
    {
        var executedTasks = tasks.Select(t => t()).ToArray();
        return await Task.WhenAll(executedTasks);
    }

    // ===== Quick Timeout Helpers =====

    /// <summary>
    /// Runs async work with a timeout, throwing TimeoutException if exceeded.
    /// </summary>
    /// <param name="work">The async work to execute.</param>
    /// <param name="timeoutMilliseconds">Timeout in milliseconds.</param>
    /// <returns>A task representing the operation.</returns>
    public static Task RunWithTimeout(Func<Task> work, int timeoutMilliseconds)
    {
        return work().WithTimeout(timeoutMilliseconds);
    }

    /// <summary>
    /// Runs async work with result and a timeout, throwing TimeoutException if exceeded.
    /// </summary>
    /// <typeparam name="T">Return type.</typeparam>
    /// <param name="work">The async work to execute.</param>
    /// <param name="timeoutMilliseconds">Timeout in milliseconds.</param>
    /// <returns>A task producing the result.</returns>
    public static Task<T> RunWithTimeout<T>(Func<Task<T>> work, int timeoutMilliseconds)
    {
        return work().WithTimeout(timeoutMilliseconds);
    }

    // ===== Quick Scheduled Tasks =====

    /// <summary>
    /// Schedules work to run after a delay.
    /// </summary>
    /// <param name="work">The work to execute.</param>
    /// <param name="delayMilliseconds">Delay in milliseconds.</param>
    /// <returns>A task that completes when the work finishes.</returns>
    public static async Task RunAfterDelay(Func<Task> work, int delayMilliseconds)
    {
        await Task.Delay(delayMilliseconds);
        await work();
    }

    /// <summary>
    /// Schedules work to run after a delay with result.
    /// </summary>
    /// <typeparam name="T">Return type.</typeparam>
    /// <param name="work">The work to execute.</param>
    /// <param name="delayMilliseconds">Delay in milliseconds.</param>
    /// <returns>A task producing the result.</returns>
    public static async Task<T> RunAfterDelay<T>(Func<Task<T>> work, int delayMilliseconds)
    {
        await Task.Delay(delayMilliseconds);
        return await work();
    }

    // ===== Quick Thread Operations =====

    /// <summary>
    /// Creates and starts a background thread that executes the given action.
    /// </summary>
    /// <param name="work">The work to execute on the background thread.</param>
    /// <returns>The created thread instance.</returns>
    public static SimpleThread RunOnThread(Action work)
    {
        var thread = new SimpleThread();
        thread.SetAsBackgroundThread(true);
        thread.Start(work);
        return thread;
    }

    /// <summary>
    /// Creates and starts a background thread with cancellation support.
    /// </summary>
    /// <param name="work">The work to execute, receives a cancellation token.</param>
    /// <returns>The created thread instance.</returns>
    public static SimpleThread RunOnThread(Action<CancellationToken> work)
    {
        var thread = new SimpleThread();
        thread.SetAsBackgroundThread(true);
        thread.Start(work);
        return thread;
    }

    // ===== Quick Pool Operations =====

    /// <summary>
    /// Creates a managed thread pool and executes multiple tasks on it.
    /// </summary>
    /// <param name="maxThreads">Maximum threads in the pool (default: CPU count).</param>
    /// <param name="tasks">Tasks to execute on the pool.</param>
    /// <returns>A task that completes when all tasks finish.</returns>
    public static async Task RunOnThreadPool(int? maxThreads, params Action[] tasks)
    {
        using var pool = new ManagedThreadPool(maxThreads);
        foreach (var task in tasks)
        {
            pool.EnqueueTask(task);
        }
        pool.WaitAll();
        return;
    }

    /// <summary>
    /// Creates a managed thread pool with CPU-count threads and executes multiple tasks.
    /// </summary>
    /// <param name="tasks">Tasks to execute on the pool.</param>
    /// <returns>A task that completes when all tasks finish.</returns>
    public static Task RunOnThreadPool(params Action[] tasks)
    {
        return RunOnThreadPool(null, tasks);
    }
}
