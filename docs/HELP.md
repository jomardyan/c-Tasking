# c-Tasking Library - Complete Documentation

**Version:** 1.0.0  
**Last Updated:** November 18, 2025  
**Status:** Release Ready  
**License:** MIT

---

## ?? Table of Contents

1. [Introduction & Quick Start](#introduction--quick-start)
2. [Installation](#installation)
3. [Core Components](#core-components)
4. [Utilities & Advanced Features](#utilities--advanced-features)
5. [Complete API Reference](#complete-api-reference)
6. [Usage Examples](#usage-examples)
7. [Best Practices](#best-practices)
8. [Common Patterns](#common-patterns)
9. [Development & Contribution](#development--contribution)
10. [Publishing & Distribution](#publishing--distribution)
11. [FAQ & Troubleshooting](#faq--troubleshooting)

---

## Introduction & Quick Start

### What is c-Tasking?

c-Tasking is a lightweight, zero-dependency C# library that simplifies threading and multitasking. It wraps .NET's native APIs in easy-to-use classes and methods, making complex threading operations straightforward and safe.

### Key Features

? **Simple Task Execution** - Run sync/async code with minimal boilerplate  
? **Thread Management** - Easy thread lifecycle and cancellation control  
? **Thread Pooling** - Automatic queuing and load balancing  
? **Task Scheduling** - One-time and repeating scheduled tasks  
? **Retry Logic** - Automatic retry with exponential backoff  
? **Batch Processing** - Concurrent batch operations  
? **LINQ Extensions** - Fluent async operation chaining  
? **Production Ready** - Full resource management and timeout handling  
? **Well Documented** - Comprehensive guides and examples  
**Zero Dependencies** - Pure .NET 10.0 implementation

### Quick Start (5 Minutes)

```csharp
using c_Tasking.Core;
using c_Tasking.Utilities;
using c_Tasking.Extensions;

// 1. Simple async task
await TaskWrapper.RunAsync(async () =>
{
    await Task.Delay(1000);
    Console.WriteLine("Task complete!");
});

// 2. Simple thread
var thread = new SimpleThread();
thread.Start(cancellationToken =>
{
    while (!cancellationToken.IsCancellationRequested)
    {
        Console.WriteLine("Working...");
        Thread.Sleep(100);
    }
});
Thread.Sleep(500);
thread.Stop(timeoutMilliseconds: 5000);

// 3. Thread pool
using var pool = new ManagedThreadPool(maxThreads: 4);
for (int i = 0; i < 10; i++)
    pool.EnqueueTask(() => DoWork());
pool.WaitAll();
pool.StopAll();

// 4. Task scheduling
using var scheduler = new TaskScheduler();
scheduler.ScheduleRepeating(() =>
    Console.WriteLine("Runs every 10 seconds"),
    intervalMilliseconds: 10000);

// 5. Retry logic
var result = await TaskRetry.ExecuteWithRetry(
    async () => await CallApiAsync(),
    maxAttempts: 3,
    initialDelayMilliseconds: 100
);

// 6. Batch processing
var batcher = new ConcurrentBatcher<int>(batchSize: 10, maxConcurrentBatches: 3);
await batcher.ProcessBatches(
    Enumerable.Range(1, 100),
    async item => { await ProcessItemAsync(item); }
);
```

---

## Installation

### Via NuGet

```bash
dotnet add package c-Tasking
```

Or in your `.csproj`:

```xml
<ItemGroup>
    <PackageReference Include="c-Tasking" Version="1.0.0" />
</ItemGroup>
```

### From Source

```bash
git clone https://github.com/jomardyan/c-Tasking.git
cd c-Tasking
dotnet build -c Release
```

### Using Local NuGet

```bash
dotnet pack -c Release -o ./nupkg
dotnet add package c-Tasking --source ./nupkg
```

### Required Dependencies

- **.NET 10.0+** (no other external dependencies)
- C# 12.0+ compatible compiler

---

## Core Components

### 1. TaskWrapper - Simple Task Execution

The most basic way to run tasks on the thread pool without threading complexity.

#### Overview

```csharp
using c_Tasking.Core;

// Sync action, run async
await TaskWrapper.Run(() => DoSyncWork());

// Sync function with return
var result = await TaskWrapper.Run(() => GetValue());

// Async action
await TaskWrapper.RunAsync(async () => await DoAsyncWork());

// Async function with return
var data = await TaskWrapper.RunAsync(async () => await GetDataAsync());

// Multiple tasks in parallel
await TaskWrapper.RunParallel(
    Task.Run(() => Work1()),
    Task.Run(() => Work2()),
    Task.Run(() => Work3())
);

// Wait with timeout
TaskWrapper.WaitWithTimeout(myTask, 5000); // throws if exceeds 5 seconds
```

#### Methods

| Method | Returns | Description |
|--------|---------|-------------|
| `Run(Action)` | Task | Execute sync code asynchronously |
| `Run<T>(Func<T>)` | Task<T> | Execute sync code with return value |
| `RunAsync(Func<Task>)` | Task | Execute async code |
| `RunAsync<T>(Func<Task<T>>)` | Task<T> | Execute async code with return value |
| `RunParallel(params Task[])` | Task | Execute multiple tasks in parallel |
| `RunParallel<T>(params Task<T>[])` | Task<T[]> | Execute tasks parallel, return results |
| `WaitWithTimeout(Task, int)` | void | Wait with timeout, throws TimeoutException |
| `WaitWithTimeout<T>(Task<T>, int)` | T | Wait with timeout, return result |
| `WaitAny(params Task[])` | Task | Wait for any task to complete |
| `WaitAny<T>(params Task<T>[])` | Task<Task<T>> | Wait for any generic task to complete |

#### When to Use

- Quick async operations
- Parallel work without complex coordination
- Simple threading needs
- Task pool execution

---

### 2. SimpleThread - Thread Management

Direct thread control with simplified lifecycle and cancellation.

#### Overview

```csharp
using c_Tasking.Core;

// Basic thread
var thread = new SimpleThread();
thread.Start(() =>
{
    for (int i = 0; i < 10; i++)
    {
        Console.WriteLine($"Step {i}");
        Thread.Sleep(100);
    }
});

thread.Join(); // Wait for completion
Console.WriteLine($"Thread ID: {thread.GetThreadId()}");
Console.WriteLine($"Is running: {thread.IsRunning}");
Console.WriteLine($"Is alive: {thread.IsAlive}");

// Thread with cancellation support
var cancelThread = new SimpleThread();
cancelThread.Start(cancellationToken =>
{
    while (!cancellationToken.IsCancellationRequested)
    {
        Console.WriteLine("Working...");
        Thread.Sleep(200);
    }
});

Thread.Sleep(1000);
cancelThread.Stop(timeoutMilliseconds: 5000); // Graceful shutdown

// Set thread properties
thread.SetPriority(ThreadPriority.Highest);
thread.SetAsBackgroundThread(false);
```

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `IsRunning` | bool | Whether thread is currently executing |
| `IsAlive` | bool | Whether underlying thread is alive |

#### Methods

| Method | Returns | Description |
|--------|---------|-------------|
| `Start(Action)` | void | Start thread with simple action |
| `Start(Action<CancellationToken>)` | void | Start with cancellation support |
| `Join(int)` | bool | Wait for thread completion |
| `Stop(int)` | void | Stop gracefully with timeout |
| `SetPriority(ThreadPriority)` | void | Set thread priority |
| `SetAsBackgroundThread(bool)` | void | Set background thread flag |
| `GetThreadId()` | int? | Get managed thread ID |

#### When to Use

- Long-running operations
- Operations needing cancellation
- Background work with lifecycle control
- When you need direct thread manipulation

---

### 3. AsyncOperation - Async Tracking

Manual control over async operations for tracking completion, failures, and results.

#### Overview

```csharp
using c_Tasking.Core;

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

var finalResult = await manualOp.WaitAsync();

// Handle errors
var errorOp = new AsyncOperation();
errorOp.SetException(new Exception("Something failed"));

// Cancel operation
var cancelOp = new AsyncOperation();
cancelOp.Cancel();

// Wait with timeout
try
{
    await resultOp.WaitAsync(timeoutMilliseconds: 2000);
}
catch (OperationCanceledException)
{
    Console.WriteLine("Operation timed out");
}
```

#### Methods

| Method | Returns | Description |
|--------|---------|-------------|
| `Create(Func<Task>)` | AsyncOperation | Create and run async operation |
| `Create<T>(Func<Task<T>>)` | AsyncOperation<T> | Create with result |
| `WaitAsync()` | Task | Wait for completion |
| `WaitAsync(int)` | Task | Wait with timeout |
| `SetResult()` / `SetResult<T>(T)` | void | Mark operation complete |
| `SetException(Exception)` | void | Mark operation failed |
| `Cancel()` | void | Cancel operation |
| `IsCompleted { get; }` | bool | Check if complete |
| `Result { get; }` | T? | Get result (generic only) |

#### When to Use

- Complex async workflows
- Manual result tracking
- Callback-based integrations
- Integration with legacy code

---

### 4. ManagedThreadPool - Thread Pool Management

Manage multiple concurrent threads with automatic queuing and load balancing.

#### Overview

```csharp
using c_Tasking.Core;

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

// Properties
int active = pool.ActiveThreadCount;
int queued = pool.QueuedTaskCount;

// Wait for all to complete
pool.WaitAll(timeoutMilliseconds: 30000);

// Graceful shutdown
pool.StopAll(timeoutMilliseconds: 5000);

// Automatic cleanup with using statement
```

#### ThreadPoolStats

| Property | Type | Description |
|----------|------|-------------|
| `MaxThreads` | int | Maximum thread count |
| `ActiveThreads` | int | Currently active threads |
| `QueuedTasks` | int | Tasks waiting in queue |
| `TotalThreads` | int | Total threads created |

#### Methods

| Method | Returns | Description |
|--------|---------|-------------|
| `EnqueueTask(Action)` | void | Queue synchronous task |
| `EnqueueAsync(Func<Task>)` | void | Queue async task |
| `WaitAll(int)` | void | Wait for all tasks to complete |
| `StopAll(int)` | void | Stop all threads gracefully |
| `GetStats()` | ThreadPoolStats | Get pool statistics |
| `ActiveThreadCount { get; }` | int | Current active threads |
| `QueuedTaskCount { get; }` | int | Current queued tasks |
| `Dispose()` | void | Cleanup resources |

#### When to Use

- Process many items concurrently
- Manage thread count
- Load balancing
- Limited resource pools

---

## Utilities & Advanced Features

### 1. TaskScheduler - Scheduled Execution

Schedule tasks to run at specific times or intervals.

#### Overview

```csharp
using c_Tasking.Utilities;

using var scheduler = new TaskScheduler();

// Run once after delay
int taskId1 = scheduler.ScheduleOnce(() =>
{
    Console.WriteLine("Runs once after 2 seconds");
}, delayMilliseconds: 2000);

// Run repeatedly
int taskId2 = scheduler.ScheduleRepeating(() =>
{
    Console.WriteLine("Runs every second");
}, intervalMilliseconds: 1000);

// Run with initial delay then repeat
int taskId3 = scheduler.ScheduleWithDelay(() =>
{
    Console.WriteLine("First after 1s, then every 2s");
}, delayMilliseconds: 1000, intervalMilliseconds: 2000);

// Async scheduling
int taskId4 = scheduler.ScheduleOnceAsync(async () =>
{
    await Task.Delay(500);
    Console.WriteLine("Async scheduled task");
}, delayMilliseconds: 1000);

// Async repeating
int taskId5 = scheduler.ScheduleRepeatingAsync(async () =>
{
    await Task.Delay(100);
    Console.WriteLine("Async every 5 seconds");
}, intervalMilliseconds: 5000);

// Cancel a task
scheduler.Cancel(taskId1);

// Cancel all tasks
scheduler.CancelAll();

// Auto-cleanup with using statement
```

#### Methods

| Method | Returns | Description |
|--------|---------|-------------|
| `ScheduleOnce(Action, int)` | int | Run once after delay |
| `ScheduleRepeating(Action, int)` | int | Run repeatedly |
| `ScheduleWithDelay(Action, int, int)` | int | Run with delay then repeat |
| `ScheduleOnceAsync(Func<Task>, int)` | int | Async run once |
| `ScheduleRepeatingAsync(Func<Task>, int)` | int | Async repeat |
| `Cancel(int)` | bool | Cancel specific task |
| `CancelAll()` | void | Cancel all tasks |
| `Dispose()` | void | Cleanup resources |

#### When to Use

- Periodic tasks
- Cleanup operations
- Heartbeats and monitoring
- Timeouts and delays

---

### 2. TaskRetry - Automatic Retry

Retry operations with exponential backoff and custom error handling.

#### Overview

```csharp
using c_Tasking.Utilities;

// Simple retry with exponential backoff
var result = await TaskRetry.ExecuteWithRetry(
    async () => await FetchDataFromApi(),
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

#### Backoff Strategy

- Attempt 1: fails immediately
- Attempt 2: waits 100ms (initialDelay)
- Attempt 3: waits 200ms (initialDelay � 2)
- Attempt 4: waits 400ms (initialDelay � 4)
- Attempt 5: waits 800ms (initialDelay � 8)
- (continues doubling until maxAttempts reached)

#### Methods

| Method | Returns | Description |
|--------|---------|-------------|
| `ExecuteWithRetry<T>(Func<Task<T>>, ...)` | Task<T> | Retry async with return |
| `ExecuteWithRetry(Func<Task>, ...)` | Task | Retry async without result |
| `Execute<T>(Func<T>, ...)` | T | Retry sync with return |
| `Execute(Func<Task>, ...)` | Task | Retry sync without result |

#### When to Use

- Network calls
- API requests
- Transient failures
- Unreliable operations

---

### 3. ConcurrentBatcher - Batch Processing

Process items in batches concurrently for efficient resource usage.

#### Overview

```csharp
using c_Tasking.Utilities;

var items = Enumerable.Range(1, 100).ToList();

// Basic batching
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
        Console.WriteLine($"Batch complete: {results.Count} items");
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

#### Methods

| Method | Returns | Description |
|--------|---------|-------------|
| `ProcessBatches<TResult>(...)` | Task | Process with results callback |
| `ProcessBatches(...)` | Task | Process without results |

#### When to Use

- Bulk operations
- API batch processing
- Data transformation
- Large dataset processing

---

### 4. TaskExtensions - LINQ-like Operations

Fluent operations on tasks for cleaner async code.

#### Overview

```csharp
using c_Tasking.Extensions;

// Success callback
await Task.Run(() => 42)
    .OnSuccess(result => Console.WriteLine($"Success: {result}"));

// Exception handler
await someTask
    .OnException(ex => Console.WriteLine($"Error: {ex.Message}"));

// Cancellation handler
await someTask
    .OnCancelled(() => Console.WriteLine("Task was cancelled"));

// Finally block
await someTask
    .Finally(() => Console.WriteLine("Task ended"));

// Transform result (Map)
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

// Wait for first completion
await TaskExtensions.WaitAnyToComplete(task1, task2, task3);
```

#### Methods

| Method | Returns | Description |
|--------|---------|-------------|
| `OnSuccess(task, callback)` | Task | Run callback on success |
| `OnSuccess<T>(task, callback)` | Task | Run callback with result |
| `OnException(task, callback)` | Task | Run callback on error |
| `OnCancelled(task, callback)` | Task | Run callback on cancel |
| `Finally(task, callback)` | Task | Run callback always |
| `Map<T,R>(task, mapper)` | Task<R> | Transform result |
| `MapAsync<T,R>(task, mapper)` | Task<R> | Async transform |
| `Chain(task, nextTask)` | Task | Chain operations |
| `Chain<T,R>(task, nextTask)` | Task<R> | Chain with result |
| `TryWait(task, ms)` | (bool, T?) | Wait with timeout |
| `IgnoreException(task)` | Task | Suppress errors |
| `IgnoreException<T>(task)` | Task<T?> | Suppress errors, return null on failure |
| `WaitAllInParallel(tasks)` | Task | All tasks in parallel |
| `WaitAnyToComplete(tasks)` | Task | Any task to complete |

#### When to Use

- Cleaner async code
- Operation chaining
- Error handling
- Result transformation

---

## Complete API Reference

### Namespaces

```csharp
using c_Tasking.Core;           // TaskWrapper, SimpleThread, AsyncOperation, ManagedThreadPool
using c_Tasking.Utilities;      // TaskScheduler, TaskRetry, ConcurrentBatcher
using c_Tasking.Extensions;     // TaskExtensions
```

### Core Classes Summary

| Class | Purpose | Lifecycle |
|-------|---------|-----------|
| `TaskWrapper` | Task execution | Static methods |
| `SimpleThread` | Thread management | Create instance |
| `AsyncOperation` | Async tracking | Create instance |
| `ManagedThreadPool` | Thread pool | IDisposable |
| `TaskScheduler` | Task scheduling | IDisposable |
| `TaskRetry` | Retry logic | Static methods |
| `ConcurrentBatcher<T>` | Batch processing | Create instance |
| `TaskExtensions` | Extension methods | Extension methods |

---

## Usage Examples

### Example 1: Download Multiple Files with Retry

```csharp
var urls = new[] { "http://...", "http://...", "http://..." };
var batcher = new ConcurrentBatcher<string>(batchSize: 5, maxConcurrentBatches: 3);

await batcher.ProcessBatches(
    urls,
    async url =>
    {
        return await TaskRetry.ExecuteWithRetry(
            async () => await DownloadFile(url),
            maxAttempts: 3,
            shouldRetry: ex => ex is HttpRequestException
        );
    },
    async results =>
    {
        await SaveResults(results);
        Console.WriteLine($"Downloaded {results.Count} files");
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

// Application running...

// Gracefully stop on shutdown
thread.Stop(timeoutMilliseconds: 10000);
```

### Example 3: Scheduled Data Sync

```csharp
using var scheduler = new TaskScheduler();

// Sync every 30 minutes
scheduler.ScheduleRepeatingAsync(async () =>
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

pool.WaitAll(timeoutMilliseconds: 60000);
pool.StopAll(timeoutMilliseconds: 5000);
```

### Example 5: Retry with Custom Logic

```csharp
var result = await TaskRetry.ExecuteWithRetry(
    async () =>
    {
        var response = await apiClient.GetAsync("https://api.example.com/data");
        return await response.Content.ReadAsStringAsync();
    },
    maxAttempts: 5,
    initialDelayMilliseconds: 500,
    shouldRetry: ex =>
    {
        // Retry on timeout or server errors (5xx)
        return ex is TimeoutException ||
               (ex is HttpRequestException hre && 
                hre.Message.Contains("5"));
    }
);
```

### Example 6: LINQ-style Async Chain

```csharp
var result = await FetchUserAsync(userId)
    .Map(user => user.Email)
    .MapAsync(async email => await SendWelcomeEmail(email))
    .OnSuccess(() => Console.WriteLine("Email sent"))
    .OnException(ex => Console.WriteLine($"Error: {ex.Message}"))
    .Finally(() => Console.WriteLine("Process complete"))
    .IgnoreException();
```

---

## Best Practices

### 1. Resource Cleanup

Always use `using` statement for `ManagedThreadPool` and `TaskScheduler`:

```csharp
using var pool = new ManagedThreadPool(4);
// Your code
// Automatic cleanup on scope exit
```

### 2. Error Handling

Wrap operations in try-catch for production:

```csharp
try
{
    var result = await TaskRetry.ExecuteWithRetry(
        async () => await Operation(),
        maxAttempts: 3,
        shouldRetry: ex => ex is not InvalidOperationException
    );
}
catch (AggregateException ae)
{
    foreach (var inner in ae.InnerExceptions)
        LogError(inner);
}
```

### 3. Timeout Management

Set appropriate timeouts based on operation type:

```csharp
// Network operations: 30 seconds
// File operations: 60 seconds
// Local operations: 5 seconds
TaskWrapper.WaitWithTimeout(task, 30000);
```

### 4. Thread Count Selection

Use `Environment.ProcessorCount` as baseline:

```csharp
var pool = new ManagedThreadPool(
    maxThreads: Environment.ProcessorCount * 2
);
```

### 5. Logging in Production

Log task progress:

```csharp
scheduler.ScheduleOnceAsync(async () =>
{
    logger.LogInformation("Scheduled task started");
    await DoWork();
    logger.LogInformation("Scheduled task completed");
}, delayMilliseconds: 1000);
```

### 6. Cancellation Support

Always check cancellation tokens:

```csharp
thread.Start(cancellationToken =>
{
    while (!cancellationToken.IsCancellationRequested)
    {
        // Do work
        if (cancellationToken.IsCancellationRequested)
            break;
    }
});
```

### 7. Resource Limits

Monitor pool statistics:

```csharp
var stats = pool.GetStats();
if (stats.QueuedTasks > 1000)
    logger.LogWarning("High queue depth: {Count}", stats.QueuedTasks);
```

---

## Common Patterns

### Fire and Forget (Safe)

```csharp
_ = Task.Run(() => DoWork()).IgnoreException();
```

### Parallel Execution

```csharp
await TaskExtensions.WaitAllInParallel(
    Task.Run(() => Work1()),
    Task.Run(() => Work2()),
    Task.Run(() => Work3())
);
```

### Sequential Chain

```csharp
await task1
    .Chain(async _ => await task2)
    .Chain(async _ => await task3);
```

### Conditional Retry

```csharp
await TaskRetry.ExecuteWithRetry(
    operation,
    maxAttempts: 3,
    shouldRetry: ex => ex is TimeoutException || 
                      ex is HttpRequestException
);
```

### Batch and Schedule

```csharp
using var scheduler = new TaskScheduler();
scheduler.ScheduleRepeatingAsync(async () =>
{
    await batcher.ProcessBatches(items, processor);
}, intervalMilliseconds: 60000);
```

### Poll Until Condition

```csharp
var timeoutAt = DateTime.UtcNow.AddSeconds(30);
while (DateTime.UtcNow < timeoutAt)
{
    if (CheckCondition())
        break;
    await Task.Delay(100);
}
```

### Background Work Shutdown

```csharp
var thread = new SimpleThread();
thread.Start(ct => BackgroundWork(ct));

// On shutdown
using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
thread.Stop(10000);
if (!thread.Join(2000))
    logger.LogWarning("Thread did not stop gracefully");
```

---

## Development & Contribution

### Getting Started

1. **Clone the repository:**
   ```bash
   git clone https://github.com/jomardyan/c-Tasking.git
   cd c-Tasking
   ```

2. **Setup development environment:**
   ```bash
   dotnet restore
   dotnet build
   dotnet test
   ```

3. **Create a feature branch:**
   ```bash
   git checkout -b feature/your-feature
   ```

### Code Style Guidelines

- Follow C# naming conventions (PascalCase for public members)
- Use meaningful variable names
- Add XML documentation comments to all public members
- Keep methods focused and reasonably sized
- Add unit tests for new features

### Example Documentation

```csharp
/// <summary>
/// Performs an important operation asynchronously.
/// </summary>
/// <param name="input">The input string to process.</param>
/// <returns>The number of items processed.</returns>
/// <exception cref="ArgumentException">Thrown when input is empty.</exception>
public async Task<int> PerformOperationAsync(string input)
{
    ValidateInput(input);
    var result = await ProcessAsync(input);
    return result;
}

private static void ValidateInput(string input)
{
    if (string.IsNullOrWhiteSpace(input))
        throw new ArgumentException("Input cannot be empty", nameof(input));
}
```

### Project Structure

```
c-Tasking/
??? Core/
?   ??? TaskWrapper.cs
?   ??? SimpleThread.cs
?   ??? AsyncOperation.cs
?   ??? ManagedThreadPool.cs
??? Utilities/
?   ??? TaskScheduler.cs
?   ??? TaskRetry.cs
?   ??? ConcurrentBatcher.cs
?   ??? AdvancedUtilities.cs
??? Extensions/
?   ??? TaskExtensions.cs
??? Examples/
?   ??? UsageExamples.cs
??? ... (documentation and config files)
```

### Pull Request Process

1. Update documentation and examples if needed
2. Add unit tests for new functionality
3. Ensure all tests pass: `dotnet test`
4. Build the package: `dotnet pack`
5. Update CHANGELOG.md
6. Reference any related issues in PR description

### Reporting Issues

When reporting issues, include:

- Clear, descriptive title
- Description of the issue
- Steps to reproduce
- Expected behavior
- Actual behavior
- Environment (OS, .NET version)
- Code sample if applicable

### Feature Requests

Feature requests welcome! Please provide:

- Clear description of the feature
- Use case and motivation
- Potential implementation approach
- Any related issues

---

## Publishing & Distribution

### Version Management

Follow Semantic Versioning (MAJOR.MINOR.PATCH):

- **1.0.0** ? Initial release
- **1.1.0** ? New features, backward compatible
- **1.0.1** ? Bug fixes, backward compatible
- **2.0.0** ? Breaking changes

### Updating Version

1. Update version in `c-Tasking.csproj`:
   ```xml
   <Version>1.1.0</Version>
   ```

2. Update `CHANGELOG.md` with changes

3. Commit and create tag:
   ```bash
   git add c-Tasking.csproj CHANGELOG.md
   git commit -m "Version 1.1.0"
   git tag v1.1.0
   git push origin v1.1.0
   ```

### Publishing to NuGet

**Automatic (GitHub Actions):**
- Create a release with tag format `v1.0.0`
- GitHub Actions automatically publishes to NuGet

**Manual Publishing:**

```bash
# Create package
dotnet pack -c Release

# Push to NuGet
dotnet nuget push "./bin/Release/c-Tasking.1.0.0.nupkg" \
  --api-key YOUR_API_KEY \
  --source https://api.nuget.org/v3/index.json
```

### Getting NuGet API Key

1. Sign in to https://www.nuget.org/
2. Go to profile settings
3. Click "Edit Profile"
4. Go to "API Keys"
5. Create new key with "Push" permissions
6. Copy the key

### Package Verification

```bash
# Wait a few minutes for indexing
dotnet package search c-Tasking

# Or visit: https://www.nuget.org/packages/c-Tasking/
```

### Pre-release Versions

```xml
<Version>1.0.0-beta.1</Version>
<Version>1.0.0-rc.1</Version>
```

---

## FAQ & Troubleshooting

### Q: Which should I use: TaskWrapper or SimpleThread?

**A:** Use `TaskWrapper` for:
- Quick async operations
- Parallel work
- Simple threading needs

Use `SimpleThread` for:
- Long-running operations
- Operations needing cancellation
- Direct thread control

### Q: How do I stop a thread gracefully?

**A:** Use the `Stop` method with a timeout:
```csharp
thread.Stop(timeoutMilliseconds: 5000); // 5 second timeout
```

The thread should check `cancellationToken.IsCancellationRequested` regularly to respond quickly.

### Q: What happens if a retry limit is exceeded?

**A:** The last exception is thrown:
```csharp
try
{
    await TaskRetry.ExecuteWithRetry(operation, maxAttempts: 3);
}
catch (Exception ex)
{
    // This is the exception from the 3rd attempt
}
```

### Q: How do I set thread priority?

**A:** Use `SetPriority`:
```csharp
thread.SetPriority(ThreadPriority.Highest);
// Options: Lowest, BelowNormal, Normal, AboveNormal, Highest
```

### Q: Can I use this with async/await?

**A:** Yes! All components support async operations:
```csharp
await TaskWrapper.RunAsync(async () => { /* async work */ });
scheduler.ScheduleRepeatingAsync(async () => { /* async work */ }, 1000);
await pool.EnqueueAsync(async () => { /* async work */ });
```

### Q: What if my task needs to access shared state?

**A:** Use proper synchronization:
```csharp
private readonly object _lock = new();
private int _counter = 0;

pool.EnqueueTask(() =>
{
    lock (_lock)
    {
        _counter++;
    }
});
```

### Q: How do I timeout a long operation?

**A:** Use `WaitWithTimeout` or `TryWait`:
```csharp
// Throws TimeoutException
TaskWrapper.WaitWithTimeout(myTask, timeoutMilliseconds: 5000);

// Returns bool
var (completed, _) = myTask.TryWait(timeoutMilliseconds: 5000);
```

### Q: Can I track task progress?

**A:** Monitor pool statistics:
```csharp
var stats = pool.GetStats();
Console.WriteLine($"Progress: {stats.ActiveThreads}/{stats.MaxThreads}");
```

### Q: What's the performance overhead?

**A:** Minimal. c-Tasking is a thin wrapper around .NET threading APIs with negligible overhead.

### Q: Is this thread-safe?

**A:** Yes, all public APIs are thread-safe. However, your own shared state needs synchronization.

### Q: Can I use this in a web application?

**A:** Yes, but consider:
- Thread pools may grow unbounded with many requests
- Use `ManagedThreadPool` with reasonable `maxThreads`
- Prefer `TaskWrapper` for request handling
- Use `TaskScheduler` for background tasks

### Q: How do I handle cancellation properly?

**A:** Regularly check the cancellation token:
```csharp
thread.Start(cancellationToken =>
{
    while (!cancellationToken.IsCancellationRequested)
    {
        // Do work
        
        // Check frequently
        if (cancellationToken.IsCancellationRequested)
            return;
        
        Thread.Sleep(100);
    }
});
```

### Q: What happens if an operation is canceled?

**A:** An `OperationCanceledException` is thrown:
```csharp
try
{
    await operation.WaitAsync(timeoutMilliseconds: 100);
}
catch (OperationCanceledException)
{
    Console.WriteLine("Operation timed out or was cancelled");
}
```

---

## Additional Resources

- **GitHub Repository:** https://github.com/jomardyan/c-Tasking
- **NuGet Package:** https://www.nuget.org/packages/c-Tasking/
- **Issues & Discussions:** https://github.com/jomardyan/c-Tasking/issues
- **.NET Documentation:** https://docs.microsoft.com/en-us/dotnet/
- **Threading Guide:** https://docs.microsoft.com/en-us/dotnet/standard/threading/

---

## Summary

c-Tasking provides a comprehensive set of threading and multitasking utilities for C# developers. With zero dependencies and a clean API, it simplifies complex threading operations while maintaining production-ready reliability and performance.

**Key Takeaways:**
- ? Use `TaskWrapper` for simple async operations
- ? Use `SimpleThread` for long-running operations
- ? Use `ManagedThreadPool` for concurrent task management
- ? Use `TaskScheduler` for scheduled work
- ? Use `TaskRetry` for fault-tolerant operations
- ? Use `ConcurrentBatcher` for bulk processing
- ? Use `TaskExtensions` for fluent async code

For more information, see individual component documentation or visit the GitHub repository.

---

**Version:** 1.0.0  
**Last Updated:** November 17, 2025  
**License:** MIT  
**Status:** Production Ready

Questions? Create an issue on GitHub or check the examples in `UsageExamples.cs`.
