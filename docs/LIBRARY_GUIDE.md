# c-Tasking Library - Comprehensive Guide

A lightweight C# library that simplifies threading and multitasking by wrapping .NET's native APIs in easy-to-use classes and methods.

**Master API Reference:** [API_REFERENCE.md](API_REFERENCE.md)

## Table of Contents

- [Installation](#installation)
- [Core Components](#core-components)
- [Usage Examples](#usage-examples)
- [Best Practices](#best-practices)
- [API Reference](#api-reference)

---

## Installation

1. Clone or download the library
2. Reference it in your project:
```csharp
using c_Tasking.Core;
using c_Tasking.Utilities;
using c_Tasking.Extensions;
```

---

## Core Components

### 1. **TaskWrapper** - Simple Task Execution
The most basic way to run tasks without worrying about .NET threading details.

```csharp
// Run synchronous code asynchronously
TaskWrapper.Run(() => 
{
    Console.WriteLine("Running on thread pool");
    Thread.Sleep(1000);
});

// Run code that returns a value
var task = TaskWrapper.Run(() => 42);
var result = task.Result;

// Run async functions
await TaskWrapper.RunAsync(async () =>
{
    await Task.Delay(500);
    Console.WriteLine("Async work done");
});

// Execute multiple tasks in parallel
TaskWrapper.RunParallel(
    Task.Run(() => Console.WriteLine("Task 1")),
    Task.Run(() => Console.WriteLine("Task 2")),
    Task.Run(() => Console.WriteLine("Task 3"))
);

// Wait with timeout
TaskWrapper.WaitWithTimeout(myTask, 5000); // throws TimeoutException if exceeds 5 seconds
```

**When to use:** Quick async operations, parallel work, simple threading needs

---

### 2. **SimpleThread** - Thread Management Made Easy
Direct thread control with simplified cancellation and lifecycle management.

```csharp
// Create and start a simple thread
var thread = new SimpleThread();
thread.Start(() =>
{
    for (int i = 0; i < 10; i++)
    {
        Console.WriteLine($"Work: {i}");
        Thread.Sleep(100);
    }
});

thread.Join(); // Wait for completion

// Thread with cancellation support
var thread = new SimpleThread();
thread.Start(cancellationToken =>
{
    while (!cancellationToken.IsCancellationRequested)
    {
        Console.WriteLine("Working...");
        Thread.Sleep(200);
    }
});

Thread.Sleep(1000);
thread.Stop(timeoutMilliseconds: 5000); // Graceful shutdown

// Set thread priority
thread.SetPriority(ThreadPriority.Highest);

// Background threads
thread.SetAsBackgroundThread(true);

// Get thread ID
int? threadId = thread.GetThreadId();
```

**When to use:** Long-running operations, operations needing cancellation, background work

---

### 3. **AsyncOperation** - Manual Async Tracking
Track async operations with manual control over completion, failures, and results.

```csharp
// Create from async function
var operation = AsyncOperation.Create(async () =>
{
    await Task.Delay(500);
    Console.WriteLine("Done");
});

await operation.WaitAsync();

// With result
var resultOp = AsyncOperation<int>.Create(async () =>
{
    await Task.Delay(500);
    return 42;
});

int result = await resultOp.WaitAsync();

// Manual operation control
var manualOp = new AsyncOperation<string>();

_ = Task.Run(async () =>
{
    await Task.Delay(1000);
    manualOp.SetResult("Completed!");
});

var result = await manualOp.WaitAsync();

// Handle errors
var errorOp = new AsyncOperation();
errorOp.SetException(new Exception("Something failed"));

// Cancel operation
var cancelOp = new AsyncOperation();
cancelOp.Cancel();
```

**When to use:** Complex async workflows, manual result tracking, callback-based integrations

---

### 4. **ManagedThreadPool** - Thread Pool Management
Manage multiple concurrent threads with automatic queuing and load balancing.

```csharp
// Create pool with max 4 concurrent threads
using var pool = new ManagedThreadPool(maxThreads: 4);

// Enqueue tasks
for (int i = 0; i < 20; i++)
{
    int taskNum = i;
    pool.EnqueueTask(() =>
    {
        Console.WriteLine($"Task {taskNum} executing");
        Thread.Sleep(1000);
    });
}

// Enqueue async work
await pool.EnqueueAsync(async () =>
{
    await Task.Delay(500);
    Console.WriteLine("Async task done");
});

// Monitor pool
var stats = pool.GetStats();
Console.WriteLine($"Active threads: {stats.ActiveThreads}");
Console.WriteLine($"Queued tasks: {stats.QueuedTasks}");
Console.WriteLine($"Max threads: {stats.MaxThreads}");

// Wait for all to complete
pool.WaitAll(timeoutMilliseconds: 30000);

// Graceful shutdown
pool.StopAll(timeoutMilliseconds: 5000);
// Automatic cleanup with using statement
```

**When to use:** Process many items concurrently, manage thread count, load balancing

---

### 5. **TaskScheduler** - Scheduled Execution
Schedule tasks to run at specific times or intervals.

```csharp
using var scheduler = new TaskScheduler();

// Run once after delay
scheduler.ScheduleOnce(() => 
{
    Console.WriteLine("Runs once after 2 seconds");
}, delayMilliseconds: 2000);

// Run repeatedly
int count = 0;
var taskId = scheduler.ScheduleRepeating(() =>
{
    count++;
    Console.WriteLine($"Runs every second: {count}");
}, intervalMilliseconds: 1000);

// Run with initial delay then repeat
scheduler.ScheduleWithDelay(() =>
{
    Console.WriteLine("First after 1s, then every 2s");
}, delayMilliseconds: 1000, intervalMilliseconds: 2000);

// Async scheduling
scheduler.ScheduleOnceAsync(async () =>
{
    await Task.Delay(500);
    Console.WriteLine("Async scheduled task");
}, delayMilliseconds: 1000);

// Cancel a task
scheduler.Cancel(taskId);

// Cancel all tasks
scheduler.CancelAll();

// Auto-cleanup with using statement
```

**When to use:** Periodic tasks, cleanup operations, heartbeats, timeouts

---

### 6. **TaskRetry** - Automatic Retry with Backoff
Retry operations with exponential backoff and custom error handling.

```csharp
// Simple retry with exponential backoff
var result = await TaskRetry.ExecuteWithRetry(
    async () =>
    {
        // Your async operation
        return await FetchDataFromApi();
    },
    maxAttempts: 5,
    initialDelayMilliseconds: 100
);

// With custom error filter
await TaskRetry.ExecuteWithRetry(
    async () => await RiskyOperation(),
    maxAttempts: 3,
    initialDelayMilliseconds: 500,
    shouldRetry: ex => ex is TimeoutException // Only retry timeouts
);

// Synchronous retry
var data = TaskRetry.Execute(
    () => ParseComplexData(),
    maxAttempts: 3,
    initialDelayMilliseconds: 200
);

// Retry without result
await TaskRetry.ExecuteWithRetry(
    async () => await SendMessage(),
    maxAttempts: 4
);
```

**Backoff times:**
- Attempt 1: fails immediately
- Attempt 2: waits 100ms
- Attempt 3: waits 200ms
- Attempt 4: waits 400ms
- Attempt 5: waits 800ms

**When to use:** Network calls, API requests, transient failures

---

### 7. **ConcurrentBatcher** - Batch Processing
Process items in batches concurrently for efficient resource usage.

```csharp
var items = Enumerable.Range(1, 100).ToList();
var batcher = new ConcurrentBatcher<int>(
    batchSize: 10,              // Process 10 items per batch
    maxConcurrentBatches: 3     // Run 3 batches in parallel
);

// Process with results
await batcher.ProcessBatches(
    items,
    async item =>
    {
        var result = await ProcessItemAsync(item);
        return result;
    },
    async results =>
    {
        Console.WriteLine($"Batch complete: {results.Count} items processed");
        await SaveResults(results);
    }
);

// Process without results
await batcher.ProcessBatches(
    items,
    async item =>
    {
        await DoSomething(item);
    },
    async () =>
    {
        Console.WriteLine("Batch processing complete");
    }
);
```

**When to use:** Bulk operations, API batch processing, data transformation

---

### 8. **TaskExtensions** - LINQ-like Task Operations
Fluent operations on tasks for cleaner code.

```csharp
// Run code when task succeeds
await Task.Run(() => 42)
    .OnSuccess(result => Console.WriteLine($"Success: {result}"));

// Handle exceptions
await someTask
    .OnException(ex => Console.WriteLine($"Error: {ex.Message}"));

// Handle cancellation
await someTask
    .OnCancelled(() => Console.WriteLine("Task was cancelled"));

// Run code regardless of outcome
await someTask
    .Finally(() => Console.WriteLine("Task ended"));

// Transform results (Map)
var doubled = await Task.Run(() => 5)
    .Map(x => x * 2);

// Async transformation
var data = await Task.Run(() => "api")
    .MapAsync(async endpoint => await GetDataFrom(endpoint));

// Chain operations
var result = await Task.Run(() => 10)
    .Chain(async num => 
    {
        await Task.Delay(100);
        return num * 2;
    });

// Try wait (returns success status)
var (completed, result) = someTask.TryWait(timeoutMs: 5000);

// Ignore exceptions
await riskyTask.IgnoreException();
var value = await riskyTaskWithResult.IgnoreException<int>();

// Parallel execution
await TaskExtensions.WaitAllInParallel(task1, task2, task3);

// Wait for first to complete
await TaskExtensions.WaitAnyToComplete(task1, task2, task3);
```

**When to use:** Cleaner async code, chaining operations, error handling

---

## Usage Examples

### Example 1: Download Multiple Files in Parallel

```csharp
var urls = new[] { "http://...", "http://...", "http://..." };
var batcher = new ConcurrentBatcher<string>(batchSize: 5, maxConcurrentBatches: 3);

await batcher.ProcessBatches(
    urls,
    async url =>
    {
        return await TaskRetry.ExecuteWithRetry(
            async () => await DownloadFile(url),
            maxAttempts: 3
        );
    },
    async results =>
    {
        await SaveResults(results);
    }
);
```

### Example 2: Long-Running Background Task

```csharp
var thread = new SimpleThread();

thread.Start(cancellationToken =>
{
    while (!cancellationToken.IsCancellationRequested)
    {
        try
        {
            PerformBackgroundWork();
            Thread.Sleep(5000);
        }
        catch (Exception ex)
        {
            LogError(ex);
        }
    }
});

// Later, gracefully stop
thread.Stop(timeoutMilliseconds: 10000);
```

### Example 3: Scheduled Data Sync

```csharp
using var scheduler = new TaskScheduler();

// Sync every 30 minutes
scheduler.ScheduleRepeating(async () =>
{
    await SyncDataWithServer();
}, intervalMilliseconds: 30 * 60 * 1000);

// Cleanup every 24 hours
scheduler.ScheduleWithDelay(
    async () => await CleanupOldData(),
    delayMilliseconds: 24 * 60 * 60 * 1000,
    intervalMilliseconds: 24 * 60 * 60 * 1000
);
```

### Example 4: Process Queue with Thread Pool

```csharp
using var pool = new ManagedThreadPool(maxThreads: Environment.ProcessorCount);

var queueItems = GetQueueItems();

foreach (var item in queueItems)
{
    pool.EnqueueTask(() =>
    {
        try
        {
            ProcessQueueItem(item);
        }
        catch (Exception ex)
        {
            LogError(ex);
        }
    });
}

pool.WaitAll();
pool.StopAll();
```

---

## Best Practices

### 1. **Resource Cleanup**
Always use `using` statement for `ManagedThreadPool` and `TaskScheduler`:
```csharp
using var pool = new ManagedThreadPool();
// Your code
// Automatic cleanup
```

### 2. **Error Handling**
Always wrap operations in try-catch for production:
```csharp
await TaskRetry.ExecuteWithRetry(
    async () => await Operation(),
    maxAttempts: 3,
    shouldRetry: ex => ex is not InvalidOperationException
);
```

### 3. **Timeout Management**
Set appropriate timeouts:
```csharp
// Network operations: 30 seconds
// File operations: 60 seconds
// Local operations: 5 seconds
TaskWrapper.WaitWithTimeout(task, 30000);
```

### 4. **Thread Count**
Use `Environment.ProcessorCount` as baseline:
```csharp
var pool = new ManagedThreadPool(Environment.ProcessorCount * 2);
```

### 5. **Logging**
Log task progress in production:
```csharp
scheduler.ScheduleOnceAsync(async () =>
{
    LogInfo("Scheduled task started");
    await DoWork();
    LogInfo("Scheduled task completed");
}, delayMilliseconds: 1000);
```

---

## API Reference

### TaskWrapper
| Method | Returns | Description |
|--------|---------|-------------|
| `Run(Action)` | Task | Run code asynchronously |
| `Run<T>(Func<T>)` | Task<T> | Run code with return value |
| `RunAsync(Func<Task>)` | Task | Run async function |
| `RunAsync<T>(Func<Task<T>>)` | Task<T> | Run async function with result |
| `WaitWithTimeout(Task, int)` | void | Wait with timeout |
| `WaitWithTimeout<T>(Task<T>, int)` | T | Wait with timeout, return result |
| `RunParallel(params Task[])` | void | Run multiple tasks parallel |
| `RunParallel<T>(params Task<T>[])` | T[] | Run tasks parallel, return results |

### SimpleThread
| Method | Returns | Description |
|--------|---------|-------------|
| `Start(Action)` | void | Start thread |
| `Start(Action<CancellationToken>)` | void | Start with cancellation |
| `Join(int)` | bool | Wait for completion |
| `Stop(int)` | void | Stop gracefully |
| `SetPriority(ThreadPriority)` | void | Set thread priority |
| `SetAsBackgroundThread(bool)` | void | Set background flag |
| `GetThreadId()` | int? | Get thread ID |

### Properties
| Property | Type | Description |
|----------|------|-------------|
| `IsRunning` | bool | Thread currently running |
| `IsAlive` | bool | Thread is alive |

---

## License

This library is provided as-is for educational and professional use.

---

## Support

For issues or questions, refer to the examples in `UsageExamples.cs` or the inline documentation in each class.
