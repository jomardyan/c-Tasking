namespace c_Tasking.Core;

/// <summary>
/// Simple wrapper for Thread operations with easy start/stop functionality.
/// </summary>
public class SimpleThread
{
    private Thread? _thread;
    private bool _isRunning;
    private readonly CancellationTokenSource _cancellationTokenSource;

    public bool IsRunning => _isRunning;
    public bool IsAlive => _thread?.IsAlive ?? false;

    public SimpleThread()
    {
        _cancellationTokenSource = new CancellationTokenSource();
    }

    /// <summary>
    /// Starts a new thread executing the given action.
    /// </summary>
    public void Start(Action action)
    {
        if (_isRunning)
            throw new InvalidOperationException("Thread is already running. Create a new instance or call Stop first.");

        _isRunning = true;
        _thread = new Thread(() =>
        {
            try
            {
                action();
            }
            finally
            {
                _isRunning = false;
            }
        })
        {
            IsBackground = false
        };

        _thread.Start();
    }

    /// <summary>
    /// Starts a new thread executing the given action with cancellation support.
    /// </summary>
    public void Start(Action<CancellationToken> action)
    {
        if (_isRunning)
            throw new InvalidOperationException("Thread is already running. Create a new instance or call Stop first.");

        _isRunning = true;
        _cancellationTokenSource.TryReset();

        _thread = new Thread(() =>
        {
            try
            {
                action(_cancellationTokenSource.Token);
            }
            finally
            {
                _isRunning = false;
            }
        })
        {
            IsBackground = false
        };

        _thread.Start();
    }

    /// <summary>
    /// Waits for the thread to complete or timeout.
    /// </summary>
    public bool Join(int timeoutMilliseconds = Timeout.Infinite)
    {
        return _thread?.Join(timeoutMilliseconds) ?? false;
    }

    /// <summary>
    /// Stops the thread gracefully using cancellation token.
    /// </summary>
    public void Stop(int timeoutMilliseconds = 5000)
    {
        if (!_isRunning)
            return;

        _cancellationTokenSource.Cancel();
        Join(timeoutMilliseconds);

        // Thread.Abort() is not supported in modern .NET
        // Graceful timeout is the best approach
    }

    /// <summary>
    /// Sets the thread priority.
    /// </summary>
    public void SetPriority(ThreadPriority priority)
    {
        if (_thread != null)
            _thread.Priority = priority;
    }

    /// <summary>
    /// Sets whether the thread is a background thread.
    /// </summary>
    public void SetAsBackgroundThread(bool isBackground)
    {
        if (_thread != null)
            _thread.IsBackground = isBackground;
    }

    /// <summary>
    /// Gets the current thread ID.
    /// </summary>
    public int? GetThreadId()
    {
        return _thread?.ManagedThreadId;
    }
}
