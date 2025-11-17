namespace c_Tasking.Core;

/// <summary>
/// Simplified thread pool manager for easy management of multiple threads.
/// </summary>
public class ManagedThreadPool : IDisposable
{
    private readonly List<SimpleThread> _threads;
    private readonly int _maxThreads;
    private readonly Queue<Action> _taskQueue;
    private readonly object _lockObject = new();
    private bool _isDisposed;

    public int ActiveThreadCount => _threads.Count(t => t.IsRunning);
    public int QueuedTaskCount => _taskQueue.Count;

    public ManagedThreadPool(int? maxThreads = null)
    {
        _maxThreads = Math.Max(1, maxThreads ?? Environment.ProcessorCount);
        _threads = new List<SimpleThread>();
        _taskQueue = new Queue<Action>();
    }

    /// <summary>
    /// Enqueues a task to be executed by an available thread.
    /// </summary>
    public void EnqueueTask(Action task)
    {
        ThrowIfDisposed();

        lock (_lockObject)
        {
            if (ActiveThreadCount < _maxThreads)
            {
                ExecuteTask(task);
            }
            else
            {
                _taskQueue.Enqueue(task);
            }
        }
    }

    /// <summary>
    /// Enqueues an async task.
    /// </summary>
    public void EnqueueAsync(Func<Task> asyncTask)
    {
        EnqueueTask(() => asyncTask().Wait());
    }

    /// <summary>
    /// Waits for all threads to complete their current tasks.
    /// </summary>
    public void WaitAll(int timeoutMilliseconds = Timeout.Infinite)
    {
        ThrowIfDisposed();

        var timeout = DateTime.UtcNow.AddMilliseconds(timeoutMilliseconds);

        while (ActiveThreadCount > 0)
        {
            if (timeoutMilliseconds != Timeout.Infinite && DateTime.UtcNow > timeout)
                throw new TimeoutException("Thread pool did not complete in time");

            Thread.Sleep(50);
        }
    }

    /// <summary>
    /// Stops all threads gracefully.
    /// </summary>
    public void StopAll(int timeoutMilliseconds = 5000)
    {
        ThrowIfDisposed();

        lock (_lockObject)
        {
            _taskQueue.Clear();
            foreach (var thread in _threads)
            {
                thread.Stop(timeoutMilliseconds);
            }
            _threads.Clear();
        }
    }

    /// <summary>
    /// Gets statistics about the thread pool.
    /// </summary>
    public ThreadPoolStats GetStats()
    {
        ThrowIfDisposed();

        return new ThreadPoolStats
        {
            MaxThreads = _maxThreads,
            ActiveThreads = ActiveThreadCount,
            QueuedTasks = QueuedTaskCount,
            TotalThreads = _threads.Count
        };
    }

    private void ExecuteTask(Action task)
    {
        var thread = new SimpleThread();
        _threads.Add(thread);

        thread.Start(() =>
        {
            try
            {
                task();
            }
            finally
            {
                ProcessNextQueuedTask();
            }
        });
    }

    private void ProcessNextQueuedTask()
    {
        lock (_lockObject)
        {
            if (_taskQueue.Count > 0)
            {
                var nextTask = _taskQueue.Dequeue();
                ExecuteTask(nextTask);
            }
        }
    }

    private void ThrowIfDisposed()
    {
        if (_isDisposed)
            throw new ObjectDisposedException(nameof(ManagedThreadPool));
    }

    public void Dispose()
    {
        if (_isDisposed)
            return;

        StopAll();
        _isDisposed = true;
        GC.SuppressFinalize(this);
    }

    ~ManagedThreadPool()
    {
        Dispose();
    }
}

/// <summary>
/// Statistics about a thread pool.
/// </summary>
public class ThreadPoolStats
{
    public int MaxThreads { get; set; }
    public int ActiveThreads { get; set; }
    public int QueuedTasks { get; set; }
    public int TotalThreads { get; set; }

    public override string ToString()
    {
        return $"Max: {MaxThreads}, Active: {ActiveThreads}, Queued: {QueuedTasks}, Total: {TotalThreads}";
    }
}
