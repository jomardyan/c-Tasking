namespace c_Tasking.Core;

/// <summary>
/// Simple wrapper for async operations with result tracking and callbacks.
/// </summary>
public class AsyncOperation
{
    private TaskCompletionSource<object?>? _tcs;
    private bool _isCompleted;

    /// <summary>
    /// Indicates whether the operation has completed (success, failure or cancellation).
    /// </summary>
    public bool IsCompleted => _isCompleted;

    /// <summary>
    /// Creates a new <see cref="AsyncOperation"/> instance that can be completed manually or via provided async action.
    /// </summary>
    public AsyncOperation()
    {
        _tcs = new TaskCompletionSource<object?>();
    }

    /// <summary>
    /// Creates and starts an async operation from an async function.
    /// </summary>
    /// <param name="asyncFunc">The asynchronous func to run.</param>
    /// <returns>An <see cref="AsyncOperation"/> instance representing the started operation.</returns>
    public static AsyncOperation Create(Func<Task> asyncFunc)
    {
        var operation = new AsyncOperation();
        operation.ExecuteAsync(asyncFunc);
        return operation;
    }

    /// <summary>
    /// Creates and starts an async operation from an async function with return value.
    /// </summary>
    /// <param name="asyncFunc">The asynchronous function that returns the operation result.</param>
    /// <returns>An <see cref="AsyncOperation{T}"/> instance representing the started operation.</returns>
    public static AsyncOperation<T> Create<T>(Func<Task<T>> asyncFunc)
    {
        var operation = new AsyncOperation<T>();
        operation.ExecuteAsync(asyncFunc);
        return operation;
    }

    /// <summary>
    /// Waits for the operation to complete.
    /// </summary>
    /// <returns>A task that completes when the operation completes.</returns>
    public async Task WaitAsync()
    {
        if (_tcs == null)
            throw new InvalidOperationException("Operation is not initialized");
        
        await _tcs.Task;
    }

    /// <summary>
    /// Waits for the operation with a timeout.
    /// </summary>
    /// <param name="timeoutMilliseconds">Timeout in milliseconds.</param>
    /// <returns>A task representing the wait operation.</returns>
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
    /// <param name="ex">Exception that caused the failure.</param>
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

    /// <summary>
    /// Executes the provided async function and completes the operation when finished.
    /// Any exceptions are captured and set on the operation.
    /// </summary>
    /// <param name="asyncFunc">The function to run.</param>
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
    /// <summary>
    /// Generic async operation wrapper exposing a result and completion state.
    /// </summary>
    public class AsyncOperation<T>
{
    private TaskCompletionSource<T>? _tcs;
    private bool _isCompleted;
    private T? _result;

    /// <summary>
    /// Indicates whether the operation has completed (success, failure or cancellation).
    /// </summary>
    public bool IsCompleted => _isCompleted;
    /// <summary>
    /// Gets the result of the operation if available.
    /// </summary>
    public T? Result => _result;

    /// <summary>
    /// Creates a new generic <see cref="AsyncOperation{T}"/> instance.
    /// </summary>
    public AsyncOperation()
    {
        _tcs = new TaskCompletionSource<T>();
    }

    /// <summary>
    /// Creates and starts an async operation from an async function with return value.
    /// </summary>
    /// <param name="asyncFunc">The asynchronous function that returns the result.</param>
    /// <returns>A started <see cref="AsyncOperation{T}"/> instance.</returns>
    public static AsyncOperation<T> Create(Func<Task<T>> asyncFunc)
    {
        var operation = new AsyncOperation<T>();
        operation.ExecuteAsync(asyncFunc);
        return operation;
    }

    /// <summary>
    /// Waits for the operation to complete.
    /// </summary>
    /// <returns>A task that resolves to the operation's result.</returns>
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
    /// <param name="timeoutMilliseconds">Timeout in milliseconds.</param>
    /// <returns>The operation result when it completes before the timeout.</returns>
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
    /// <param name="result">The result to set on the operation.</param>
    public void SetResult(T? result)
    {
        _result = result;
        _tcs?.TrySetResult(result!);
        _isCompleted = true;
    }

    /// <summary>
    /// Marks the operation as failed with an exception.
    /// </summary>
    /// <param name="ex">Exception that caused the failure.</param>
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

    /// <summary>
    /// Executes the async function and sets the result when complete.
    /// Exceptions are captured and applied to the operation state.
    /// </summary>
    /// <param name="asyncFunc">The asynchronous function that produces the result.</param>
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
