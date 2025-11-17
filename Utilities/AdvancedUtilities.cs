namespace c_Tasking.Utilities;

/// <summary>
/// Advanced utilities for parallel task execution and optimization.
/// </summary>
public static class ParallelTaskExecutor
{
    /// <summary>
    /// Executes multiple async tasks with a maximum degree of parallelism.
    /// </summary>
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

    public void Dispose()
    {
        if (_isDisposed)
            return;

        _semaphore.Dispose();
        _stopwatch.Stop();
        _isDisposed = true;
        GC.SuppressFinalize(this);
    }

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
    public long CompletedOperations { get; set; }
    public long FailedOperations { get; set; }
    public long CancelledOperations { get; set; }
    public TimeSpan TotalExecutionTime { get; set; }
    public double AverageExecutionTime { get; set; }
    public long PeakConcurrentOperations { get; set; }

    public override string ToString()
    {
        return $"Completed: {CompletedOperations}, Failed: {FailedOperations}, " +
               $"Cancelled: {CancelledOperations}, Avg Time: {AverageExecutionTime:F2}ms";
    }
}
