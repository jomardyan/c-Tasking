namespace c_Tasking.Examples;

using c_Tasking.Core;
using c_Tasking.Utilities;
using c_Tasking.Extensions;

/// <summary>
/// Examples showing how to use the c-Tasking library.
/// </summary>
public class UsageExamples
{
    // ============ TaskWrapper Examples ============

    /// <summary>
    /// Demonstrates using the TaskWrapper helper for synchronous and asynchronous tasks.
    /// </summary>
    public static void ExampleTaskWrapper()
    {
        Console.WriteLine("=== TaskWrapper Examples ===\n");

        // Simple async task
        var task = TaskWrapper.Run(() =>
        {
            Console.WriteLine("Task running on thread pool");
            Thread.Sleep(1000);
        });

        task.Wait();
        Console.WriteLine("Task completed\n");

        // Task with return value
        var resultTask = TaskWrapper.Run(() =>
        {
            return 42;
        });

        var result = resultTask.Result;
        Console.WriteLine($"Task returned: {result}\n");

        // Async function
        var asyncTask = TaskWrapper.RunAsync(async () =>
        {
            await Task.Delay(500);
            Console.WriteLine("Async task completed");
        });

        asyncTask.Wait();
        Console.WriteLine();

        // Parallel execution
        var tasks = new Task[]
        {
            TaskWrapper.Run(() => { Console.WriteLine("Task 1"); Thread.Sleep(500); }),
            TaskWrapper.Run(() => { Console.WriteLine("Task 2"); Thread.Sleep(500); }),
            TaskWrapper.Run(() => { Console.WriteLine("Task 3"); Thread.Sleep(500); })
        };

        TaskWrapper.RunParallel(tasks);
        Console.WriteLine("All parallel tasks completed\n");
    }

    // ============ SimpleThread Examples ============

    /// <summary>
    /// Demonstrates SimpleThread usage including cancellation and priority.
    /// </summary>
    public static void ExampleSimpleThread()
    {
        Console.WriteLine("=== SimpleThread Examples ===\n");

        // Simple thread
        var thread = new SimpleThread();
        thread.Start(() =>
        {
            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine($"Thread running: {i}");
                Thread.Sleep(100);
            }
        });

        thread.Join();
        Console.WriteLine("Thread finished\n");

        // Thread with cancellation
        var cancellableThread = new SimpleThread();
        cancellableThread.Start(cancellationToken =>
        {
            for (int i = 0; i < 10; i++)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    Console.WriteLine("Thread cancelled!");
                    break;
                }

                Console.WriteLine($"Working: {i}");
                Thread.Sleep(200);
            }
        });

        Thread.Sleep(500);
        cancellableThread.Stop();
        Console.WriteLine();

        // Thread priority
        var highPriorityThread = new SimpleThread();
        highPriorityThread.SetPriority(ThreadPriority.Highest);
        highPriorityThread.Start(() => Console.WriteLine("High priority thread"));
        highPriorityThread.Join();
        Console.WriteLine();
    }

    // ============ AsyncOperation Examples ============

    /// <summary>
    /// Demonstrates AsyncOperation usage and manual completion patterns.
    /// </summary>
    public static async Task ExampleAsyncOperation()
    {
        Console.WriteLine("=== AsyncOperation Examples ===\n");

        // Simple async operation
        var operation = AsyncOperation.Create(async () =>
        {
            await Task.Delay(500);
            Console.WriteLine("Async operation completed");
        });

        await operation.WaitAsync();
        Console.WriteLine();

        // Async operation with result
        var resultOperation = AsyncOperation<int>.Create(async () =>
        {
            await Task.Delay(500);
            return 123;
        });

        var opResult = await resultOperation.WaitAsync();
        Console.WriteLine($"Operation result: {opResult}\n");

        // Manual async operation
        var manualOp = new AsyncOperation();
        _ = Task.Run(async () =>
        {
            await Task.Delay(500);
            manualOp.SetResult();
        });

        await manualOp.WaitAsync();
        Console.WriteLine("Manual operation completed\n");
    }

    // ============ ManagedThreadPool Examples ============

    /// <summary>
    /// Demonstrates a managed thread pool enqueuing and tracking tasks.
    /// </summary>
    public static void ExampleManagedThreadPool()
    {
        Console.WriteLine("=== ManagedThreadPool Examples ===\n");

        using var pool = new ManagedThreadPool(maxThreads: 3);

        // Enqueue multiple tasks
        for (int i = 0; i < 10; i++)
        {
            int taskNum = i;
            pool.EnqueueTask(() =>
            {
                Console.WriteLine($"Task {taskNum} started");
                Thread.Sleep(500);
                Console.WriteLine($"Task {taskNum} completed");
            });
        }

        var stats = pool.GetStats();
        Console.WriteLine($"Pool stats: {stats}\n");

        pool.WaitAll();
        Console.WriteLine("All tasks completed\n");
    }

    // ============ TaskScheduler Examples ============

    /// <summary>
    /// Demonstrates task scheduling with one-time and repeating tasks.
    /// </summary>
    public static void ExampleTaskScheduler()
    {
        Console.WriteLine("=== TaskScheduler Examples ===\n");

        using var scheduler = new TaskScheduler();

        // One-time task
        scheduler.ScheduleOnce(() => Console.WriteLine("One-time task executed"), delayMilliseconds: 500);

        // Repeating task
        int count = 0;
        int taskId = 0;
        taskId = scheduler.ScheduleRepeating(() =>
        {
            count++;
            Console.WriteLine($"Repeating task: {count}");
            if (count >= 3)
            {
                scheduler.Cancel(taskId);
            }
        }, intervalMilliseconds: 200);

        Thread.Sleep(1000);
        Console.WriteLine();
    }

    // ============ TaskRetry Examples ============

    /// <summary>
    /// Demonstrates retrying operations with exponential backoff.
    /// </summary>
    public static async Task ExampleTaskRetry()
    {
        Console.WriteLine("=== TaskRetry Examples ===\n");

        int attempts = 0;

        // Retry with exponential backoff
        var result = await TaskRetry.ExecuteWithRetry(
            async () =>
            {
                attempts++;
                Console.WriteLine($"Attempt {attempts}");

                if (attempts < 3)
                    throw new Exception("Failed");

                await Task.Delay(100);
                return "Success";
            },
            maxAttempts: 5,
            initialDelayMilliseconds: 100
        );

        Console.WriteLine($"Final result: {result}\n");
    }

    // ============ ConcurrentBatcher Examples ============

    /// <summary>
    /// Demonstrates concurrent batch processing with result callbacks.
    /// </summary>
    public static async Task ExampleConcurrentBatcher()
    {
        Console.WriteLine("=== ConcurrentBatcher Examples ===\n");

        var items = Enumerable.Range(1, 15).ToList();
        var batcher = new ConcurrentBatcher<int>(batchSize: 5, maxConcurrentBatches: 2);

        await batcher.ProcessBatches(
            items,
            async item =>
            {
                Console.WriteLine($"Processing item: {item}");
                await Task.Delay(200);
                return item * 2;
            },
            async results =>
            {
                Console.WriteLine($"Batch complete, results: {string.Join(", ", results)}");
                await Task.CompletedTask;
            }
        );

        Console.WriteLine();
    }

    // ============ TaskExtensions Examples ============

    /// <summary>
    /// Demonstrates various task extension methods (Map, Chain, OnSuccess, etc.).
    /// </summary>
    public static async Task ExampleTaskExtensions()
    {
        Console.WriteLine("=== TaskExtensions Examples ===\n");

        // Create a completed task with result
        var completedTask = Task.FromResult(42);

        // OnSuccess callback
        await completedTask.OnSuccess(result =>
        {
            Console.WriteLine($"Task succeeded with result: {result}");
        });

        // Map transformation
        var transformed = await completedTask.Map(x => x * 2);
        Console.WriteLine($"Transformed result: {transformed}\n");

        // Chain operations
        var chained = await Task.FromResult(10).Chain(async num =>
        {
            await Task.Delay(100);
            return num * 2;
        });

        Console.WriteLine($"Chained result: {chained}\n");
    }

    // ============ Complete Example ============

    /// <summary>
    /// Runs a complete real-world example combining thread pool, scheduler, batcher and retry logic.
    /// </summary>
    public static async Task CompleteExample()
    {
        Console.WriteLine("=== Complete Real-World Example ===\n");

        using var pool = new ManagedThreadPool(maxThreads: 4);
        using var scheduler = new TaskScheduler();

        // Simulate processing multiple data items with retries and scheduling
        var dataItems = Enumerable.Range(1, 8).ToList();
        var batcher = new ConcurrentBatcher<int>(batchSize: 2, maxConcurrentBatches: 2);

        var processingTask = batcher.ProcessBatches(
            dataItems,
            async item =>
            {
                return await TaskRetry.ExecuteWithRetry(
                    async () =>
                    {
                        Console.WriteLine($"Processing item {item}");
                        await Task.Delay(300);
                        return item * 10;
                    },
                    maxAttempts: 3
                );
            },
            async results =>
            {
                Console.WriteLine($"Batch processed: {results.Count} items");
                await Task.CompletedTask;
            }
        );

        await processingTask;

        // Schedule a summary task
        scheduler.ScheduleOnce(() =>
        {
            Console.WriteLine("\nProcessing complete!");
        }, delayMilliseconds: 500);     



        Thread.Sleep(1001); // Wait for scheduled tasks to complete
    }
}
