namespace c_Tasking.Core;

/// <summary>
/// Simple wrapper for executing actions in tasks with minimal configuration.
/// </summary>
public class TaskWrapper
{
    /// <summary>
    /// Runs an action asynchronously.
    /// </summary>
    public static Task Run(Action action)
    {
        return Task.Run(action);
    }

    /// <summary>
    /// Runs a function asynchronously and returns its result.
    /// </summary>
    public static Task<T> Run<T>(Func<T> function)
    {
        return Task.Run(function);
    }

    /// <summary>
    /// Runs an async function asynchronously.
    /// </summary>
    public static Task RunAsync(Func<Task> asyncAction)
    {
        return Task.Run(asyncAction);
    }

    /// <summary>
    /// Runs an async function asynchronously and returns its result.
    /// </summary>
    public static async Task<T> RunAsync<T>(Func<Task<T>> asyncFunction)
    {
        return await asyncFunction();
    }

    /// <summary>
    /// Waits for a task to complete with a timeout. Throws TimeoutException if timeout is exceeded.
    /// </summary>
    public static void WaitWithTimeout(Task task, int timeoutMilliseconds)
    {
        if (!task.Wait(timeoutMilliseconds))
        {
            throw new TimeoutException($"Task did not complete within {timeoutMilliseconds}ms");
        }
    }

    /// <summary>
    /// Waits for a task to complete with a timeout and returns the result.
    /// </summary>
    public static T WaitWithTimeout<T>(Task<T> task, int timeoutMilliseconds)
    {
        if (!task.Wait(timeoutMilliseconds))
        {
            throw new TimeoutException($"Task did not complete within {timeoutMilliseconds}ms");
        }
        return task.Result;
    }

    /// <summary>
    /// Runs multiple tasks in parallel and waits for all to complete.
    /// </summary>
    public static void RunParallel(params Task[] tasks)
    {
        Task.WaitAll(tasks);
    }

    /// <summary>
    /// Runs multiple tasks in parallel and returns their results.
    /// </summary>
    public static T[] RunParallel<T>(params Task<T>[] tasks)
    {
        Task.WaitAll(tasks);
        return tasks.Select(t => t.Result).ToArray();
    }

    /// <summary>
    /// Waits for the first task to complete among many.
    /// </summary>
    public static Task WaitAny(params Task[] tasks)
    {
        return Task.WhenAny(tasks);
    }

    /// <summary>
    /// Waits for the first task to complete and returns the completed task.
    /// </summary>
    public static Task<Task<T>> WaitAny<T>(params Task<T>[] tasks)
    {
        return Task.WhenAny(tasks);
    }
}
