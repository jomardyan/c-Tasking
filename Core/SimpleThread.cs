using System.Threading;

namespace c_Tasking.Core;

/// <summary>
/// Simple wrapper for Thread operations with easy start/stop functionality.
/// </summary>
public class SimpleThread
{
    private Thread? _thread;
    private bool _isRunning;
    private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

    /// <summary>
    /// Indicates whether the thread is currently running.
    /// </summary>
    public bool IsRunning => _isRunning;
    /// <summary>
    /// Indicates whether the underlying Thread instance is alive.
    /// </summary>
    public bool IsAlive => _thread?.IsAlive ?? false;

    /// <summary>
    /// Creates a new <see cref="SimpleThread"/> instance.
    /// </summary>
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

        var startedEvent = new ManualResetEventSlim(false);

        _thread = new Thread(() =>
        {
            // Signal that the thread has begun execution before invoking the action
            startedEvent.Set();
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

        // Wait briefly for the thread to begin executing so callers relying on immediate start observe started state
        // Do not block indefinitely; give a reasonable timeout
        startedEvent.Wait(1000);
    }

    /// <summary>
    /// Starts a new thread executing the given action with cancellation support.
    /// </summary>
    public void Start(Action<CancellationToken> action)
    {
        if (_isRunning)
            throw new InvalidOperationException("Thread is already running. Create a new instance or call Stop first.");

        _isRunning = true;
        // Reset cancellation token source for this start
        if (_cancellationTokenSource.IsCancellationRequested)
        {
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = new CancellationTokenSource();
        }

        var startedEvent = new ManualResetEventSlim(false);

        _thread = new Thread(() =>
        {
            // Signal that the thread has begun execution before invoking the action
            startedEvent.Set();
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

        // Wait briefly for the thread to begin executing so callers relying on immediate start observe started state
        // Do not block indefinitely; give a reasonable timeout
        startedEvent.Wait(1000);
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
        if (_thread != null)
        {
            // Wait for thread to respect cancellation and exit
            _thread.Join(timeoutMilliseconds);
        }

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
