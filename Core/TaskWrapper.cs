namespace c_Tasking.Core;

/// <summary>
/// Simple wrapper for executing actions in tasks with minimal configuration.
/// </summary>
public class TaskWrapper
{
    /// <summary>
    /// Runs an action asynchronously.
    /// </summary>
    /// <param name="action">The synchronous action to run on a background thread.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public static Task Run(Action action)
    {
        return Task.Run(action);
    }

    /// <summary>
    /// Runs a function asynchronously and returns its result.
    /// </summary>
    /// <param name="function">The function that returns a value to be executed asynchronously.</param>
    /// <returns>A <see cref="Task{T}"/> representing the result of the asynchronous operation.</returns>
    public static Task<T> Run<T>(Func<T> function)
    {
        return Task.Run(function);
    }

    /// <summary>
    /// Runs an async function asynchronously.
    /// </summary>
    /// <param name="asyncAction">The asynchronous function to run.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public static Task RunAsync(Func<Task> asyncAction)
    {
        return Task.Run(asyncAction);
    }

    /// <summary>
    /// Runs an async function asynchronously and returns its result.
    /// </summary>
    /// <param name="asyncFunction">The asynchronous function that produces a value.</param>
    /// <returns>A <see cref="Task{T}"/> that completes when the function completes.</returns>
    public static async Task<T> RunAsync<T>(Func<Task<T>> asyncFunction)
    {
        return await asyncFunction();
    }

    /// <summary>
    /// Waits for a task to complete with a timeout. Throws <see cref="TimeoutException"/> if timeout is exceeded.
    /// </summary>
    /// <param name="task">The task to wait on.</param>
    /// <param name="timeoutMilliseconds">Timeout in milliseconds.</param>
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
    /// <param name="task">The task to wait on.</param>
    /// <param name="timeoutMilliseconds">Timeout in milliseconds.</param>
    /// <returns>The task result.</returns>
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
    /// <param name="tasks">Tasks to run in parallel.</param>
    public static void RunParallel(params Task[] tasks)
    {
        Task.WaitAll(tasks);
    }

    /// <summary>
    /// Runs multiple tasks in parallel and returns their results.
    /// </summary>
    /// <param name="tasks">Tasks returning values to run in parallel.</param>
    /// <returns>Array of results in the same order as the tasks.</returns>
    public static T[] RunParallel<T>(params Task<T>[] tasks)
    {
        Task.WaitAll(tasks);
        return tasks.Select(t => t.Result).ToArray();
    }

    /// <summary>
    /// Waits for the first task to complete among many.
    /// </summary>
    /// <param name="tasks">Tasks to wait on.</param>
    /// <returns>A <see cref="Task"/> that completes when any task completes.</returns>
    public static Task WaitAny(params Task[] tasks)
    {
        return Task.WhenAny(tasks);
    }

    /// <summary>
    /// Waits for the first task to complete and returns the completed task.
    /// </summary>
    /// <param name="tasks">Tasks to wait on.</param>
    /// <returns>A task wrapping the completed task.</returns>
    public static Task<Task<T>> WaitAny<T>(params Task<T>[] tasks)
    {
        return Task.WhenAny(tasks);
    }
}
