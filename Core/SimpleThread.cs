using System;
using System.Threading;

namespace c_Tasking.Core;

/// <summary>
/// Simple wrapper for Thread operations with easy start/stop functionality.
/// </summary>
public class SimpleThread
{
    private Thread? _thread;
    private volatile bool _isRunning;
    private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
    private readonly object _threadLock = new object();

    /// <summary>
    /// Indicates whether the thread is currently running.
    /// </summary>
    public bool IsRunning => _isRunning;

    /// <summary>
    /// Indicates whether the underlying Thread instance is alive.
    /// </summary>
 
    {
        get
        {
            lock (_threadLock)
            {
                return _thread?.IsAlive ?? false;
            }
        }
    }

    /// <summary>
    /// Creates a new <see cref="SimpleThread"/> instance.
    /// </summary>
    public SimpleThread()
    {
        _cancellationTokenSource = new CancellationTokenSource();
        public void Start(Action<CancellationToken> action)
        {
            if (_isRunning)
                throw new InvalidOperationException("Thread is already running. Create a new instance or call Stop first.");
            _isRunning = true;
            ErrorHandler.Instance.LogMessage("Thread start (with cancellation)", "SimpleThread.Start");
            // Reset cancellation token source for this start
            if (_cancellationTokenSource.IsCancellationRequested)
            {
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = new CancellationTokenSource();
            }
            var startedEvent = new ManualResetEventSlim(false);
            var t = new Thread(() =>
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
                    ErrorHandler.Instance.LogMessage("Thread exited", "SimpleThread.ThreadExit");
                }
            }) { IsBackground = false };
            lock (_threadLock)
            {
                _thread = t;
                _thread.Start();
            }
            // Wait briefly for the thread to begin executing so callers relying on immediate start observe started state
            startedEvent.Wait(1000);
        }
                        lock (_threadLock)
                        {
                            _thread = t;
                            _thread.Start();
                        }
                        startedEvent.Wait(1000);
                    }
            {
                action(_cancellationTokenSource.Token);
            }
            finally
            {
                _isRunning = false;
                ErrorHandler.Instance.LogMessage("Thread exited", "SimpleThread.ThreadExit");
            }
        })
        {
            IsBackground = false
        };
<<<<<<< HEAD

        _thread.Start();

        // Wait briefly for the thread to begin executing so callers relying on immediate start observe started state
        // Do not block indefinitely; give a reasonable timeout
        startedEvent.Wait(1000);
=======
        lock (_threadLock)
        {
            _thread = t;
            _thread.Start();
        }
        // spin-wait briefly to allow the thread to begin executing (reduces test flakes)
        swStart = System.Diagnostics.Stopwatch.StartNew();
        while (!_thread.IsAlive && swStart.ElapsedMilliseconds < 50)
            Thread.Yield();
>>>>>>> ff69970 (feat: Enhance error handling and logging across core components; add usage documentation)
    }

    /// <summary>
    /// Waits for the thread to complete or timeout.
    /// </summary>
    public bool Join(int timeoutMilliseconds = Timeout.Infinite)
    {
        Thread? t;
        lock (_threadLock)
        {
            t = _thread;
        }
        if (t == null)
            return !_isRunning;
        return t.Join(timeoutMilliseconds);
    }

    /// <summary>
    /// Stops the thread gracefully using cancellation token.
    /// </summary>
    public void Stop(int timeoutMilliseconds = 5000)
    {
        if (!_isRunning)
            return;
        // Mark requested stop immediately to reflect calling Stop semantics.
        _isRunning = false;
        ErrorHandler.Instance.LogMessage("Stop invoked", "SimpleThread.Stop");
        _cancellationTokenSource.Cancel();
        ErrorHandler.Instance.LogMessage($"Cancellation requested: {_cancellationTokenSource.IsCancellationRequested}", "SimpleThread.Stop");
        Thread? t;
        lock (_threadLock)
        {
            t = _thread;
        }
        if (t != null)
        {
            // wake sleeping thread if any (Thread.Sleep) so it can observe cancellation
            try
            {
                t.Interrupt();
                ErrorHandler.Instance.LogMessage("Thread interrupted", "SimpleThread.Stop");
            }
            catch { }
            // wait in short increments for thread to exit, more reliable under contention
            var sw = System.Diagnostics.Stopwatch.StartNew();
            while (t.IsAlive && sw.ElapsedMilliseconds < timeoutMilliseconds)
            {
                // attempt to wake the thread frequently
                try { t.Interrupt(); } catch { }
                t.Join(10);
                // small yield to give scheduler time
                Thread.Yield();
            }
            if (t.IsAlive)
                ErrorHandler.Instance.LogMessage("Stop timeout waiting for thread to exit", "SimpleThread.Stop");
        }
        // mark logical 'running' state as false after stop request irrespective of underlying thread
        _isRunning = false;
        ErrorHandler.Instance.LogMessage($"Stop finished; _isRunning={_isRunning}; IsAlive={_thread?.IsAlive}", "SimpleThread.Stop");

        // Thread.Abort() is not supported in modern .NET
        // Graceful timeout is the best approach
    }

    /// <summary>
    /// Sets the thread priority.
    /// </summary>
    public void SetPriority(ThreadPriority priority)
    {
        Thread? t;
        lock (_threadLock)
        {
            t = _thread;
        }
        if (t != null)
            t.Priority = priority;
    }

    /// <summary>
    /// Sets whether the thread is a background thread.
    /// </summary>
    public void SetAsBackgroundThread(bool isBackground)
    {
        Thread? t;
        lock (_threadLock)
        {
            t = _thread;
        }
        if (t != null)
            t.IsBackground = isBackground;
    }

    /// <summary>
    /// Gets the current thread ID.
    /// </summary>
    public int? GetThreadId()
    {
        lock (_threadLock)
        {
            return _thread?.ManagedThreadId;
        }
    }
}
