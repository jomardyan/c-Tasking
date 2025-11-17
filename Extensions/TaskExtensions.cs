namespace c_Tasking.Extensions;

/// <summary>
/// Extension methods for Task operations to simplify common patterns.
/// </summary>
public static class TaskExtensions
{
    /// <summary>
    /// Executes a callback when the task completes, regardless of success or failure.
    /// </summary>
    /// <param name="task">The task to monitor.</param>
    /// <param name="callback">The callback executed upon completion.</param>
    /// <returns>The original task.</returns>
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
    /// <param name="task">The task to monitor.</param>
    /// <param name="callback">Callback executed on successful completion.</param>
    /// <returns>The original task.</returns>
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
    /// <typeparam name="T">Return type of the task.</typeparam>
    /// <param name="task">The task to monitor.</param>
    /// <param name="callback">Callback invoked with the task result.</param>
    /// <returns>The original task.</returns>
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
    /// <param name="task">The monitored task.</param>
    /// <param name="callback">Callback invoked with the exception that caused the failure.</param>
    /// <returns>The original task.</returns>
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
    /// <param name="task">The monitored task.</param>
    /// <param name="callback">Callback executed when the task is cancelled.</param>
    /// <returns>The original task.</returns>
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
    /// Waits for a task with a timeout, returns true if completed, false if timed out.
    /// </summary>
    /// <param name="task">The task to wait on.</param>
    /// <param name="timeoutMilliseconds">Timeout in milliseconds.</param>
    /// <returns>True if the task completes within the timeout; otherwise false.</returns>
    public static bool TryWait(this Task task, int timeoutMilliseconds)
    {
        return task.Wait(timeoutMilliseconds);
    }

    /// <summary>
    /// Waits for task with a timeout, returns result and completion status.
    /// </summary>
    /// <param name="task">The task to wait on.</param>
    /// <param name="timeoutMilliseconds">Timeout in milliseconds.</param>
    /// <returns>Tuple with completion status and result if completed.</returns>
    public static (bool completed, T? result) TryWait<T>(this Task<T> task, int timeoutMilliseconds)
    {
        var completed = task.Wait(timeoutMilliseconds);
        return (completed, completed ? task.Result : default);
    }

    /// <summary>
    /// Transforms the result of a task.
    /// </summary>
    /// <typeparam name="T">Type of the input task result.</typeparam>
    /// <typeparam name="TResult">Type of the mapped result.</typeparam>
    /// <param name="task">The source task.</param>
    /// <param name="mapper">Mapper function to transform the result.</param>
    /// <returns>A task producing the transformed result.</returns>
    public static async Task<TResult> Map<T, TResult>(this Task<T> task, Func<T, TResult> mapper)
    {
        var result = await task;
        return mapper(result);
    }

    /// <summary>
    /// Transforms the result of a task asynchronously.
    /// </summary>
    /// <typeparam name="T">Type of the input task result.</typeparam>
    /// <typeparam name="TResult">Type of the mapped result.</typeparam>
    /// <param name="task">The source task.</param>
    /// <param name="mapper">Async mapper function to transform the result.</param>
    /// <returns>A task producing the transformed result.</returns>
    public static async Task<TResult> MapAsync<T, TResult>(this Task<T> task, Func<T, Task<TResult>> mapper)
    {
        var result = await task;
        return await mapper(result);
    }

    /// <summary>
    /// Chains multiple tasks in sequence.
    /// </summary>
    /// <param name="task">The initial task whose completion triggers the next one.</param>
    /// <param name="nextTask">The function that returns the next task to execute.</param>
    /// <returns>A task representing the chained execution.</returns>
    public static async Task Chain(this Task task, Func<Task> nextTask)
    {
        await task;
        await nextTask();
    }

    /// <summary>
    /// Chains multiple tasks in sequence with result passing.
    /// </summary>
    /// <typeparam name="T">Type of the input task result.</typeparam>
    /// <typeparam name="TResult">Type of the next task result.</typeparam>
    /// <param name="task">The initial task.</param>
    /// <param name="nextTask">The function to produce the next task using the input result.</param>
    /// <returns>A task producing the result of the chained operation.</returns>
    public static async Task<TResult> Chain<T, TResult>(this Task<T> task, Func<T, Task<TResult>> nextTask)
    {
        var result = await task;
        return await nextTask(result);
    }

    /// <summary>
    /// Silently catches and ignores exceptions from a task.
    /// </summary>
    /// <param name="task">The task whose exception should be ignored.</param>
    /// <returns>A task that completes when the given task has completed.</returns>
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
    /// <typeparam name="T">Type of the task result.</typeparam>
    /// <param name="task">The input task.</param>
    /// <returns>The task result if successful, otherwise default.</returns>
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
    /// <param name="tasks">Tasks to run in parallel.</param>
    /// <returns>A task that completes when all tasks have completed.</returns>
    public static Task WaitAllInParallel(params Task[] tasks)
    {
        return Task.WhenAll(tasks);
    }

    /// <summary>
    /// Waits for the first task to complete.
    /// </summary>
    /// <param name="tasks">Tasks to wait on.</param>
    /// <returns>A task that completes when any of the tasks completes.</returns>
    public static Task WaitAnyToComplete(params Task[] tasks)
    {
        return Task.WhenAny(tasks);
    }
}


