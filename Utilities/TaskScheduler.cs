namespace c_Tasking.Utilities;

/// <summary>
/// Simple task scheduler for executing tasks at specific times or intervals.
/// </summary>
public class TaskScheduler : IDisposable
{
    private readonly Dictionary<int, Timer> _timers;
    private int _nextId;
    private bool _isDisposed;

    /// <summary>
    /// Creates a new <see cref="TaskScheduler"/> instance used to schedule recurring and one-time tasks.
    /// </summary>
    public TaskScheduler()
    {
        _timers = new Dictionary<int, Timer>();
        _nextId = 1;
    }

    /// <summary>
    /// Schedules a task to run once after a delay.
    /// </summary>
    /// <param name="task">The action to run.</param>
    /// <param name="delayMilliseconds">Delay in milliseconds before running the action.</param>
    /// <returns>The scheduled task identifier.</returns>
    public int ScheduleOnce(Action task, int delayMilliseconds)
    {
        ThrowIfDisposed();

        var id = _nextId++;
        var timer = new Timer(_ =>
        {
            task();
            RemoveTimer(id);
        }, null, delayMilliseconds, Timeout.Infinite);

        _timers[id] = timer;
        return id;
    }

    /// <summary>
    /// Schedules a task to run repeatedly at a fixed interval.
    /// </summary>
    /// <param name="task">The action to run.</param>
    /// <param name="intervalMilliseconds">The repeat interval in milliseconds.</param>
    /// <returns>The scheduled task identifier.</returns>
    public int ScheduleRepeating(Action task, int intervalMilliseconds)
    {
        ThrowIfDisposed();

        var id = _nextId++;
        var timer = new Timer(_ => task(), null, intervalMilliseconds, intervalMilliseconds);

        _timers[id] = timer;
        return id;
    }

    /// <summary>
    /// Schedules a task to run with an initial delay and then repeatedly at an interval.
    /// </summary>
    /// <param name="task">The action to run.</param>
    /// <param name="delayMilliseconds">Initial delay in milliseconds before first run.</param>
    /// <param name="intervalMilliseconds">Interval in milliseconds for recurring runs.</param>
    /// <returns>The scheduled task identifier.</returns>
    public int ScheduleWithDelay(Action task, int delayMilliseconds, int intervalMilliseconds)
    {
        ThrowIfDisposed();

        var id = _nextId++;
        var timer = new Timer(_ => task(), null, delayMilliseconds, intervalMilliseconds);

        _timers[id] = timer;
        return id;
    }

    /// <summary>
    /// Schedules an async task to run once after a delay.
    /// </summary>
    /// <param name="task">Async function to schedule.</param>
    /// <param name="delayMilliseconds">Delay in milliseconds before the scheduled execution.</param>
    /// <returns>The scheduled task identifier.</returns>
    public int ScheduleOnceAsync(Func<Task> task, int delayMilliseconds)
    {
        return ScheduleOnce(() => task().Wait(), delayMilliseconds);
    }

    /// <summary>
    /// Schedules an async task to run repeatedly at a fixed interval.
    /// </summary>
    /// <param name="task">Async function to schedule repeatedly.</param>
    /// <param name="intervalMilliseconds">Repeat interval in milliseconds.</param>
    /// <returns>The scheduled task identifier.</returns>
    public int ScheduleRepeatingAsync(Func<Task> task, int intervalMilliseconds)
    {
        return ScheduleRepeating(() => task().Wait(), intervalMilliseconds);
    }

    /// <summary>
    /// Cancels a scheduled task using its task id.
    /// </summary>
    /// <param name="taskId">The id returned when the task was scheduled.</param>
    /// <returns>True if the task was found and cancelled, otherwise false.</returns>
    public bool Cancel(int taskId)
    {
        ThrowIfDisposed();
        return RemoveTimer(taskId);
    }

    /// <summary>
    /// Cancels all scheduled tasks.
    /// </summary>
    public void CancelAll()
    {
        ThrowIfDisposed();

        foreach (var id in _timers.Keys.ToList())
        {
            RemoveTimer(id);
        }
    }

    private bool RemoveTimer(int id)
    {
        if (_timers.TryGetValue(id, out var timer))
        {
            timer.Dispose();
            _timers.Remove(id);
            return true;
        }
        return false;
    }

    private void ThrowIfDisposed()
    {
        if (_isDisposed)
            throw new ObjectDisposedException(nameof(TaskScheduler));
    }

    /// <summary>
    /// Disposes the scheduler and cancels any scheduled tasks.
    /// </summary>
    public void Dispose()
    {
        if (_isDisposed)
            return;

        CancelAll();
        _isDisposed = true;
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Finalizer ensures the scheduler is disposed if Dispose is not called.
    /// </summary>
    ~TaskScheduler()
    {
        Dispose();
    }
}
