namespace c_Tasking.Extensions;

/// <summary>
/// Extension methods for Task operations to simplify common patterns.
/// </summary>
public static class TaskExtensions
{
    /// <summary>
    /// Executes a callback when the task completes, regardless of success or failure.
    /// </summary>
    public static Task Finally(this Task task, Action callback)
    {
        return task.ContinueWith(_ =>
        {
            callback();
        });
    }

    /// <summary>
    /// Executes a callback when the task completes successfully.
    /// </summary>
    public static Task OnSuccess(this Task task, Action callback)
    {
        return task.ContinueWith(_ =>
        {
            if (task.IsCompletedSuccessfully)
            {
                callback();
            }
        });
    }

    /// <summary>
    /// Executes a callback when a generic task completes successfully, passing the result.
    /// </summary>
    public static Task OnSuccess<T>(this Task<T> task, Action<T> callback)
    {
        return task.ContinueWith(_ =>
        {
            if (task.IsCompletedSuccessfully)
            {
                callback(task.Result);
            }
        });
    }

    /// <summary>
    /// Executes a callback when the task fails with an exception.
    /// </summary>
    public static Task OnException(this Task task, Action<Exception> callback)
    {
        return task.ContinueWith(_ =>
        {
            if (task.IsFaulted && task.Exception != null)
            {
                callback(task.Exception.InnerException ?? task.Exception);
            }
        });
    }

    /// <summary>
    /// Executes a callback when the task is cancelled.
    /// </summary>
    public static Task OnCancelled(this Task task, Action callback)
    {
        return task.ContinueWith(_ =>
        {
            if (task.IsCanceled)
            {
                callback();
            }
        });
    }

    /// <summary>
    /// Waits for task with a timeout, returns true if completed, false if timed out.
    /// </summary>
    public static bool TryWait(this Task task, int timeoutMilliseconds)
    {
        return task.Wait(timeoutMilliseconds);
    }

    /// <summary>
    /// Waits for task with a timeout, returns result and completion status.
    /// </summary>
    public static (bool completed, T? result) TryWait<T>(this Task<T> task, int timeoutMilliseconds)
    {
        var completed = task.Wait(timeoutMilliseconds);
        return (completed, completed ? task.Result : default);
    }

    /// <summary>
    /// Transforms the result of a task.
    /// </summary>
    public static async Task<TResult> Map<T, TResult>(this Task<T> task, Func<T, TResult> mapper)
    {
        var result = await task;
        return mapper(result);
    }

    /// <summary>
    /// Transforms the result of a task asynchronously.
    /// </summary>
    public static async Task<TResult> MapAsync<T, TResult>(this Task<T> task, Func<T, Task<TResult>> mapper)
    {
        var result = await task;
        return await mapper(result);
    }

    /// <summary>
    /// Chains multiple tasks in sequence.
    /// </summary>
    public static async Task Chain(this Task task, Func<Task> nextTask)
    {
        await task;
        await nextTask();
    }

    /// <summary>
    /// Chains multiple tasks in sequence with result passing.
    /// </summary>
    public static async Task<TResult> Chain<T, TResult>(this Task<T> task, Func<T, Task<TResult>> nextTask)
    {
        var result = await task;
        return await nextTask(result);
    }

    /// <summary>
    /// Silently catches and ignores exceptions from a task.
    /// </summary>
    public static async Task IgnoreException(this Task task)
    {
        try
        {
            await task;
        }
        catch { }
    }

    /// <summary>
    /// Silently catches and ignores exceptions from a task, returns default on error.
    /// </summary>
    public static async Task<T?> IgnoreException<T>(this Task<T> task)
    {
        try
        {
            return await task;
        }
        catch
        {
            return default;
        }
    }

    /// <summary>
    /// Runs multiple tasks in parallel and waits for all to complete.
    /// </summary>
    public static Task WaitAllInParallel(params Task[] tasks)
    {
        return Task.WhenAll(tasks);
    }

    /// <summary>
    /// Waits for the first task to complete.
    /// </summary>
    public static Task WaitAnyToComplete(params Task[] tasks)
    {
        return Task.WhenAny(tasks);
    }
}


