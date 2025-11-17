namespace c_Tasking.Core;

/// <summary>
/// Simple wrapper for async operations with result tracking and callbacks.
/// </summary>
public class AsyncOperation
{
    private TaskCompletionSource<object?>? _tcs;
    private bool _isCompleted;

    public bool IsCompleted => _isCompleted;

    public AsyncOperation()
    {
        _tcs = new TaskCompletionSource<object?>();
    }

    /// <summary>
    /// Creates and starts an async operation from an async function.
    /// </summary>
    public static AsyncOperation Create(Func<Task> asyncFunc)
    {
        var operation = new AsyncOperation();
        operation.ExecuteAsync(asyncFunc);
        return operation;
    }

    /// <summary>
    /// Creates and starts an async operation from an async function with return value.
    /// </summary>
    public static AsyncOperation<T> Create<T>(Func<Task<T>> asyncFunc)
    {
        var operation = new AsyncOperation<T>();
        operation.ExecuteAsync(asyncFunc);
        return operation;
    }

    /// <summary>
    /// Waits for the operation to complete.
    /// </summary>
    public async Task WaitAsync()
    {
        if (_tcs == null)
            throw new InvalidOperationException("Operation is not initialized");
        
        await _tcs.Task;
    }

    /// <summary>
    /// Waits for the operation with a timeout.
    /// </summary>
    public async Task WaitAsync(int timeoutMilliseconds)
    {
        if (_tcs == null)
            throw new InvalidOperationException("Operation is not initialized");

        using var cts = new CancellationTokenSource(timeoutMilliseconds);
        await _tcs.Task.ConfigureAwait(false);
    }

    /// <summary>
    /// Marks the operation as completed successfully.
    /// </summary>
    public void SetResult()
    {
        _tcs?.TrySetResult(null);
        _isCompleted = true;
    }

    /// <summary>
    /// Marks the operation as failed with an exception.
    /// </summary>
    public void SetException(Exception ex)
    {
        _tcs?.TrySetException(ex);
        _isCompleted = true;
    }

    /// <summary>
    /// Cancels the operation.
    /// </summary>
    public void Cancel()
    {
        _tcs?.TrySetCanceled();
        _isCompleted = true;
    }

    public async void ExecuteAsync(Func<Task> asyncFunc)
    {
        try
        {
            await asyncFunc();
            SetResult();
        }
        catch (Exception ex)
        {
            SetException(ex);
        }
    }
}

/// <summary>
/// Generic wrapper for async operations with result tracking and callbacks.
/// </summary>
public class AsyncOperation<T>
{
    private TaskCompletionSource<T>? _tcs;
    private bool _isCompleted;
    private T? _result;

    public bool IsCompleted => _isCompleted;
    public T? Result => _result;

    public AsyncOperation()
    {
        _tcs = new TaskCompletionSource<T>();
    }

    /// <summary>
    /// Creates and starts an async operation from an async function with return value.
    /// </summary>
    public static AsyncOperation<T> Create(Func<Task<T>> asyncFunc)
    {
        var operation = new AsyncOperation<T>();
        operation.ExecuteAsync(asyncFunc);
        return operation;
    }

    /// <summary>
    /// Waits for the operation to complete.
    /// </summary>
    public async Task<T?> WaitAsync()
    {
        if (_tcs == null)
            throw new InvalidOperationException("Operation is not initialized");

        _result = await _tcs.Task;
        return _result;
    }

    /// <summary>
    /// Waits for the operation with a timeout.
    /// </summary>
    public async Task<T?> WaitAsync(int timeoutMilliseconds)
    {
        if (_tcs == null)
            throw new InvalidOperationException("Operation is not initialized");

        using var cts = new CancellationTokenSource(timeoutMilliseconds);
        _result = await _tcs.Task.ConfigureAwait(false);
        return _result;
    }

    /// <summary>
    /// Marks the operation as completed with a result.
    /// </summary>
    public void SetResult(T? result)
    {
        _result = result;
        _tcs?.TrySetResult(result!);
        _isCompleted = true;
    }

    /// <summary>
    /// Marks the operation as failed with an exception.
    /// </summary>
    public void SetException(Exception ex)
    {
        _tcs?.TrySetException(ex);
        _isCompleted = true;
    }

    /// <summary>
    /// Cancels the operation.
    /// </summary>
    public void Cancel()
    {
        _tcs?.TrySetCanceled();
        _isCompleted = true;
    }

    public async void ExecuteAsync(Func<Task<T>> asyncFunc)
    {
        try
        {
            var result = await asyncFunc();
            SetResult(result);
        }
        catch (Exception ex)
        {
            SetException(ex);
        }
    }
}
