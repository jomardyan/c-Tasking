namespace c_Tasking.Utilities;

/// <summary>
/// Advanced utilities for parallel task execution and optimization.
/// </summary>
public static class ParallelTaskExecutor
{
    /// <summary>
    /// Executes multiple async tasks with a maximum degree of parallelism.
    /// </summary>
    /// <summary>
    /// Executes multiple async tasks with a maximum degree of parallelism.
    /// </summary>
    /// <param name="items">Items to run the action for.</param>
    /// <param name="asyncAction">Async action that executes per item.</param>
    /// <param name="maxDegreeOfParallelism">Maximum parallelism for execution.</param>
    public static async Task ExecuteParallelAsync<T>(
        IEnumerable<T> items,
        Func<T, Task> asyncAction,
        int maxDegreeOfParallelism = 4)
    {
        using var semaphore = new SemaphoreSlim(maxDegreeOfParallelism);
        var tasks = items.Select(async item =>
        {
            await semaphore.WaitAsync();
            try
            {
                await asyncAction(item);
            }
            finally
            {
                semaphore.Release();
            }
        });

        await Task.WhenAll(tasks);
    }

    /// <summary>
    /// Executes multiple async functions that return results with controlled parallelism.
    /// </summary>
    /// <summary>
    /// Executes multiple async functions in parallel returning results with a maximum degree of parallelism.
    /// </summary>
    /// <param name="asyncFunctions">Async functions to invoke in parallel.</param>
    /// <param name="maxDegreeOfParallelism">Maximum concurrency.</param>
    /// <returns>An enumerable of results for each function invocation.</returns>
    public static async Task<IEnumerable<TResult>> ExecuteParallelAsync<TResult>(
        IEnumerable<Func<Task<TResult>>> asyncFunctions,
        int maxDegreeOfParallelism = 4)
    {
        using var semaphore = new SemaphoreSlim(maxDegreeOfParallelism);
        var tasks = asyncFunctions.Select(async func =>
        {
            await semaphore.WaitAsync();
            try
            {
                return await func();
            }
            finally
            {
                semaphore.Release();
            }
        });

        return await Task.WhenAll(tasks);
    }
}

/// <summary>
/// Timeout utilities for operations with deadline management.
/// </summary>
public static class TimeoutManager
{
    /// <summary>
    /// Executes an async operation with a deadline timeout.
    /// </summary>
    /// <summary>
    /// Executes an async operation using a cancellation token with a configured timeout.
    /// </summary>
    /// <param name="operation">The operation that accepts a cancellation token.</param>
    /// <param name="timeout">Timeout for the operation.</param>
    public static async Task<T> ExecuteWithDeadlineAsync<T>(
        Func<CancellationToken, Task<T>> operation,
        TimeSpan timeout)
    {
        using var cts = new CancellationTokenSource(timeout);
        return await operation(cts.Token);
    }

    /// <summary>
    /// Executes an async operation with a deadline timeout.
    /// </summary>
    /// <summary>
    /// Executes an async operation using a cancellation token with a configured timeout.
    /// </summary>
    /// <param name="operation">The operation that accepts a cancellation token.</param>
    /// <param name="timeout">Timeout for the operation.</param>
    public static async Task ExecuteWithDeadlineAsync(
        Func<CancellationToken, Task> operation,
        TimeSpan timeout)
    {
        using var cts = new CancellationTokenSource(timeout);
        await operation(cts.Token);
    }

    /// <summary>
    /// Polls an async condition until true or timeout.
    /// </summary>
    /// <summary>
    /// Polls until a boolean condition returns true or timeout is reached.
    /// </summary>
    /// <param name="condition">Predicate returning true to stop polling.</param>
    /// <param name="timeout">Timeout for overall polling duration.</param>
    /// <param name="pollInterval">Interval between polls.</param>
    /// <returns>Returns true if the condition becomes true within the timeout.</returns>
    public static async Task<bool> PollUntilAsync(
        Func<Task<bool>> condition,
        TimeSpan timeout,
        TimeSpan pollInterval)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        while (stopwatch.Elapsed < timeout)
        {
            if (await condition())
                return true;

            await Task.Delay(pollInterval);
        }
        return false;
    }
}

/// <summary>
/// Throttling utilities to rate-limit operations.
/// </summary>
public class TaskThrottler : IDisposable
{
    private readonly SemaphoreSlim _semaphore;
    private readonly System.Diagnostics.Stopwatch _stopwatch;
    private bool _isDisposed;

    /// <summary>
    /// Creates a throttler that allows maxOperations within timeWindow.
    /// </summary>
    public TaskThrottler(int maxOperations, TimeSpan timeWindow)
    {
        _semaphore = new SemaphoreSlim(maxOperations, maxOperations);
        _stopwatch = System.Diagnostics.Stopwatch.StartNew();
    }

    /// <summary>
    /// Throttles an async operation.
    /// </summary>
    /// <param name="operation">The operation to throttle.</param>
    /// <returns>The result of the operation.</returns>
    public async Task<T> ThrottleAsync<T>(Func<Task<T>> operation)
    {
        ThrowIfDisposed();
        await _semaphore.WaitAsync();
        try
        {
            return await operation();
        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <summary>
    /// Throttles an async operation without return value.
    /// </summary>
    /// <param name="operation">The operation to throttle.</param>
    public async Task ThrottleAsync(Func<Task> operation)
    {
        ThrowIfDisposed();
        await _semaphore.WaitAsync();
        try
        {
            await operation();
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private void ThrowIfDisposed()
    {
        if (_isDisposed)
            throw new ObjectDisposedException(nameof(TaskThrottler));
    }

    /// <summary>
    /// Disposes the throttler and releases any internal resources.
    /// </summary>
    public void Dispose()
    {
        if (_isDisposed)
            return;

        _semaphore.Dispose();
        _stopwatch.Stop();
        _isDisposed = true;
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Finalizer ensures resources are cleaned up if Dispose was not called.
    /// </summary>
    ~TaskThrottler()
    {
        Dispose();
    }
}

/// <summary>
/// Provides utilities for managing and waiting on task collections.
/// </summary>
public static class TaskWaiter
{
    /// <summary>
    /// Waits for all tasks to complete, with detailed timeout information.
    /// </summary>
    public static async Task<bool> WaitAllWithTimeoutAsync(
        IEnumerable<Task> tasks,
        TimeSpan timeout)
    {
        var cts = new CancellationTokenSource(timeout);
        try
        {
            await Task.WhenAll(tasks);
            return true;
        }
        catch (OperationCanceledException)
        {
            return false;
        }
    }

    /// <summary>
    /// Waits for at least one task to complete successfully.
    /// </summary>
    public static async Task<Task> WaitForAnyCompletionAsync(
        params Task[] tasks)
    {
        return await Task.WhenAny(tasks);
    }

    /// <summary>
    /// Waits for a task with a completion callback.
    /// </summary>
    public static Task WithCompletionCallback(
        this Task task,
        Action onComplete)
    {
        return task.ContinueWith(_ => onComplete());
    }
}

/// <summary>
/// Statistics and monitoring for task execution.
/// </summary>
public class ExecutionMetrics
{
    /// <summary>
    /// Number of operations that completed successfully.
    /// </summary>
    public long CompletedOperations { get; set; }
    /// <summary>
    /// Number of operations that failed.
    /// </summary>
    public long FailedOperations { get; set; }
    /// <summary>
    /// Number of operations that were cancelled.
    /// </summary>
    public long CancelledOperations { get; set; }
    /// <summary>
    /// Total cumulative execution time for all tracked operations.
    /// </summary>
    public TimeSpan TotalExecutionTime { get; set; }
    /// <summary>
    /// Average execution time for tracked operations in milliseconds.
    /// </summary>
    public double AverageExecutionTime { get; set; }
    /// <summary>
    /// Peak number of concurrent operations observed.
    /// </summary>
    public long PeakConcurrentOperations { get; set; }

    /// <summary>
    /// Returns a readable summary of the current execution metrics.
    /// </summary>
    public override string ToString()
    {
        return $"Completed: {CompletedOperations}, Failed: {FailedOperations}, " +
               $"Cancelled: {CancelledOperations}, Avg Time: {AverageExecutionTime:F2}ms";
    }
}
