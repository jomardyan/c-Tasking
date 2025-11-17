namespace c_Tasking.Utilities;

/// <summary>
/// Simple task scheduler for executing tasks at specific times or intervals.
/// </summary>
public class TaskScheduler : IDisposable
{
    private readonly Dictionary<int, Timer> _timers;
    private int _nextId;
    private bool _isDisposed;

    public TaskScheduler()
    {
        _timers = new Dictionary<int, Timer>();
        _nextId = 1;
    }

    /// <summary>
    /// Schedules a task to run once after a delay.
    /// </summary>
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
    public int ScheduleOnceAsync(Func<Task> task, int delayMilliseconds)
    {
        return ScheduleOnce(() => task().Wait(), delayMilliseconds);
    }

    /// <summary>
    /// Schedules an async task to run repeatedly at a fixed interval.
    /// </summary>
    public int ScheduleRepeatingAsync(Func<Task> task, int intervalMilliseconds)
    {
        return ScheduleRepeating(() => task().Wait(), intervalMilliseconds);
    }

    /// <summary>
    /// Cancels a scheduled task.
    /// </summary>
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

    public void Dispose()
    {
        if (_isDisposed)
            return;

        CancelAll();
        _isDisposed = true;
        GC.SuppressFinalize(this);
    }

    ~TaskScheduler()
    {
        Dispose();
    }
}
