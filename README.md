# c-Tasking - Modern C# Threading & Multitasking Library

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![NuGet Version](https://img.shields.io/badge/NuGet-v1.0.0-blue)](https://www.nuget.org/packages/c-Tasking/)
[![.NET](https://img.shields.io/badge/.NET-8.0+-512BD4.svg)](https://dotnet.microsoft.com/)

A lightweight, intuitive C# library that simplifies threading, async operations, and multitasking. Wrap complex .NET threading patterns into simple, easy-to-use APIs.

## 📚 Documentation

For detailed documentation, visit the [docs](docs/) folder:

- **[Quick Start & API Reference](docs/QUICK_REFERENCE.md)** - Get started quickly
- **[API Reference](docs/API_REFERENCE.md)** - Consolidated Application API reference
- **[Comprehensive Guide](docs/LIBRARY_GUIDE.md)** - Detailed component documentation
- **[Getting Started Guide](docs/PROJECT_SETUP.md)** - Development setup
- **[Contributing Guide](docs/CONTRIBUTING.md)** - How to contribute
- **[Full Documentation Index](docs/INDEX.md)** - Complete documentation directory

## 🎯 Features

- **Simple Task Execution** - Run synchronous and asynchronous code with minimal configuration
- **Thread Management** - Easy thread creation, lifecycle management, and cancellation
- **Async Operations** - Manual async operation tracking with result management
- **Thread Pool** - Managed thread pool with automatic queuing and load balancing
- **Task Scheduling** - Schedule tasks to run at specific times or intervals
- **Retry Logic** - Automatic retry with exponential backoff for resilient operations
- **Batch Processing** - Concurrent batch processing with configurable concurrency
- **Task Extensions** - LINQ-like extensions for cleaner async code
- **Full .NET 8.0+ Support** - Modern C# with nullable reference types
- **Zero Dependencies** - No external NuGet dependencies required

## 📦 Installation

### NuGet Package Manager
```bash
dotnet add package c-Tasking
```

### Package Manager Console
```powershell
Install-Package c-Tasking
```

### .csproj
```xml
<ItemGroup>
    <PackageReference Include="c-Tasking" Version="1.0.0" />
</ItemGroup>
```

## 🚀 Quick Start

### Basic Task Execution

```csharp
using c_Tasking.Core;

// Run sync code asynchronously
await TaskWrapper.RunAsync(() => 
{
    Console.WriteLine("Running on thread pool");
});

// Run code with return value
var result = await TaskWrapper.RunAsync(async () =>
{
    await Task.Delay(1000);
    return 42;
});
## VS Code - Test Explorer Setup

If you use VS Code, install the recommended extensions and open the Tests tab to run and debug xUnit tests:

- Install the *C#* extension: `ms-dotnettools.csharp`
- Install the *Test Explorer UI* extension: `hbenl.test-explorer`
- Install the *Dotnet Test Explorer* extension: `formulahendry.dotnet-test-explorer`

The workspace includes recommended extensions and a settings file - after installation, build the solution and the tests should appear in the Test Explorer panel:

```bash
dotnet restore
dotnet build
# open Tests panel in VS Code — the Test Explorer will discover xUnit tests
```
```

### Simple Threading

```csharp
using c_Tasking.Core;

var thread = new SimpleThread();

// Start a background thread
thread.Start(() =>
{
    for (int i = 0; i < 10; i++)
    {
        Console.WriteLine($"Iteration: {i}");
        Thread.Sleep(100);
    }
});

// Wait for completion or timeout
thread.Join(timeoutMilliseconds: 5000);
```

### Thread Pool Management

```csharp
using c_Tasking.Core;

using var pool = new ManagedThreadPool(maxThreads: 4);

// Enqueue multiple tasks
for (int i = 0; i < 100; i++)
{
    pool.EnqueueTask(() => DoWork());
}

// Wait for all to complete
pool.WaitAll();
```

### Scheduled Tasks

```csharp
using c_Tasking.Utilities;

using var scheduler = new TaskScheduler();

// Run once after 5 seconds
scheduler.ScheduleOnce(() => 
{
    Console.WriteLine("Runs once");
}, delayMilliseconds: 5000);

// Run every 10 seconds
scheduler.ScheduleRepeating(() =>
{
    Console.WriteLine("Repeating task");
}, intervalMilliseconds: 10000);
```

### Retry with Exponential Backoff

```csharp
using c_Tasking.Utilities;

var result = await TaskRetry.ExecuteWithRetry(
    async () => await CallUnstableApi(),
    maxAttempts: 5,
    initialDelayMilliseconds: 100,
    shouldRetry: ex => ex is TimeoutException // Custom retry logic
);
```

### Batch Processing

```csharp
using c_Tasking.Utilities;

var items = Enumerable.Range(1, 1000).ToList();
var batcher = new ConcurrentBatcher<int>(
    batchSize: 100,
    maxConcurrentBatches: 3
);

await batcher.ProcessBatches(
    items,
    async item => await ProcessItem(item),
    async results => await SaveBatch(results)
);
```

### Task Extensions - LINQ-style

```csharp
using c_Tasking.Extensions;

var result = await Task.FromResult(42)
    .OnSuccess(value => Console.WriteLine($"Success: {value}"))
    .Map(x => x * 2)
    .MapAsync(async x => await FetchData(x));

// Chain operations
await task1
    .Chain(async _ => await task2)
    .Chain(async _ => await task3);

// Handle errors gracefully
await riskyOperation.IgnoreException();
```


## Usage & Comparison

This section provides a concise mapping and guidance comparing common patterns using native .NET primitives and the c-Tasking helpers. For full details and examples, see `docs/USAGE_AND_COMPARISON.md`.

-### Component Mapping

- TaskWrapper ⇄ Task / Task.Run / Task.WhenAll / Task.WhenAny
- SimpleThread ⇄ Thread (System.Threading) / long-running background threads
- AsyncOperation ⇄ TaskCompletionSource / manual Task-based coordination
- ManagedThreadPool ⇄ ThreadPool / TPL (Task Parallel Library)
- TaskScheduler ⇄ System.Threading.Timer / Task.Delay / external schedulers (Quartz.NET, Hangfire)
- TaskRetry ⇄ Manual try/catch + Task.Delay or resilience libs like Polly
- ConcurrentBatcher ⇄ Parallel.ForEach / SemaphoreSlim / Dataflow
- TaskExtensions ⇄ Common Task chaining and continuation patterns


-### c-Tasking specifics & notes

- Exception handling: Most helpers log exceptions via `ErrorHandler` by default — be explicit if you need exceptions propagated.
- Disposable resources: `TaskScheduler` and `ManagedThreadPool` implement `IDisposable` — use `using var` or call `Dispose()`.
- Threads: `SimpleThread` defaults to `IsBackground = false` — call `SetAsBackgroundThread(true)` if desired.
- Both sync and async overloads exist across utilities — prefer the overload matching your code path.


-### When to use .NET primitives

- When you need max performance, low-level control, or tighter resource management.
- When you rely on TPL Dataflow, Parallel constructs, or enterprise schedulers.


-### When to use c-Tasking

- When you prefer intention-revealing, small helpers to reduce boilerplate.
- When you want built-in features like retries, scheduling, and bounded concurrency without adding dependencies.


-### Migration tips

- Replace ad-hoc `Thread` usage with `SimpleThread` for lifecycle and cancellation.
- Use `TaskWrapper` for concise fire-and-forget and basic parallel operations.
- Keep Polly or other libraries if you need advanced resilience policies; `TaskRetry` is handy for small projects.

For a full comparison and runnable samples, see `docs/USAGE_AND_COMPARISON.md` and `Examples/UsageExamples.cs`.


## 🏗️ Project Structure

```
c-Tasking/
├── Core/                      # Core threading components
│   ├── TaskWrapper.cs
│   ├── SimpleThread.cs
│   ├── AsyncOperation.cs
│   └── ManagedThreadPool.cs
├── Utilities/                 # Utility classes
│   ├── TaskScheduler.cs
│   ├── TaskRetry.cs
│   ├── ConcurrentBatcher.cs
│   └── AdvancedUtilities.cs
├── Extensions/                # Extension methods
│   └── TaskExtensions.cs
├── Examples/                  # Usage examples
│   └── UsageExamples.cs
├── docs/                      # Documentation
├── README.md                  # This file
├── LICENSE                    # MIT License
└── c-Tasking.csproj          # Project file
```

## 📖 Core Components

See [Comprehensive Guide](docs/LIBRARY_GUIDE.md) for detailed documentation on:

- **TaskWrapper** - Simple task execution wrapper
- **SimpleThread** - Thread management with cancellation
- **AsyncOperation** - Async operation tracking
- **ManagedThreadPool** - Thread pool with auto-queuing
- **TaskScheduler** - Task scheduling utilities
- **TaskRetry** - Retry logic with exponential backoff
- **ConcurrentBatcher** - Batch processing
- **TaskExtensions** - LINQ-style task extensions

## 🎓 Common Patterns

### Resilient API Calls

```csharp
var data = await TaskRetry.ExecuteWithRetry(
    async () => await apiClient.GetDataAsync(),
    maxAttempts: 3,
    initialDelayMilliseconds: 100,
    shouldRetry: ex => ex is HttpRequestException
);
```

### Background Processing

```csharp
using var scheduler = new TaskScheduler();

scheduler.ScheduleRepeating(async () =>
{
    await ProcessBackgroundJobs();
}, intervalMilliseconds: 60000); // Every minute
```

### Bulk Data Processing

```csharp
using var pool = new ManagedThreadPool(Environment.ProcessorCount);

foreach (var item in largeDataset)
{
    pool.EnqueueTask(() => ProcessItem(item));
}

pool.WaitAll();
```

## 🤝 Contributing

Contributions are welcome! See [Contributing Guide](docs/CONTRIBUTING.md) for details.

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙌 Support

For issues, questions, or suggestions:
- Open an issue on [GitHub](https://github.com/jomardyan/c-Tasking/issues)
- Check the [documentation](docs/)
- Review [examples](Examples/UsageExamples.cs)

## 📈 Changelog

See [CHANGELOG.md](docs/CHANGELOG.md) for version history and updates.

---

**Made with ❤️ for the .NET community**

## 🎯 Features

- **Simple Task Execution** - Run synchronous and asynchronous code with minimal configuration
- **Thread Management** - Easy thread creation, lifecycle management, and cancellation
- **Async Operations** - Manual async operation tracking with result management
- **Thread Pool** - Managed thread pool with automatic queuing and load balancing
- **Task Scheduling** - Schedule tasks to run at specific times or intervals
- **Retry Logic** - Automatic retry with exponential backoff for resilient operations
- **Batch Processing** - Concurrent batch processing with configurable concurrency
- **Task Extensions** - LINQ-like extensions for cleaner async code
- **Full .NET 8.0+ Support** - Modern C# with nullable reference types
- **Zero Dependencies** - No external NuGet dependencies required

## 📦 Installation

### NuGet Package Manager
```bash
dotnet add package c-Tasking
```

### Package Manager Console
```powershell
Install-Package c-Tasking
```

### .csproj
```xml
<ItemGroup>
    <PackageReference Include="c-Tasking" Version="1.0.0" />
</ItemGroup>
```

## 🚀 Quick Start

### Basic Task Execution

```csharp
using c_Tasking.Core;

// Run sync code asynchronously
await TaskWrapper.RunAsync(() => 
{
    Console.WriteLine("Running on thread pool");
});

// Run code with return value
var result = await TaskWrapper.RunAsync(async () =>
{
    await Task.Delay(1000);
    return 42;
});
```

### Simple Threading

```csharp
using c_Tasking.Core;

var thread = new SimpleThread();

// Start a background thread
thread.Start(() =>
{
    for (int i = 0; i < 10; i++)
    {
        Console.WriteLine($"Iteration: {i}");
        Thread.Sleep(100);
    }
});

// Wait for completion or timeout
thread.Join(timeoutMilliseconds: 5000);
```

### Thread Pool Management

```csharp
using c_Tasking.Core;

using var pool = new ManagedThreadPool(maxThreads: 4);

// Enqueue multiple tasks
for (int i = 0; i < 100; i++)
{
    pool.EnqueueTask(() => DoWork());
}

// Wait for all to complete
pool.WaitAll();
```

### Scheduled Tasks

```csharp
using c_Tasking.Utilities;

using var scheduler = new TaskScheduler();

// Run once after 5 seconds
scheduler.ScheduleOnce(() => 
{
    Console.WriteLine("Runs once");
}, delayMilliseconds: 5000);

// Run every 10 seconds
scheduler.ScheduleRepeating(() =>
{
    Console.WriteLine("Repeating task");
}, intervalMilliseconds: 10000);
```

### Retry with Exponential Backoff

```csharp
using c_Tasking.Utilities;

var result = await TaskRetry.ExecuteWithRetry(
    async () => await CallUnstableApi(),
    maxAttempts: 5,
    initialDelayMilliseconds: 100,
    shouldRetry: ex => ex is TimeoutException // Custom retry logic
);
```

### Batch Processing

```csharp
using c_Tasking.Utilities;

var items = Enumerable.Range(1, 1000).ToList();
var batcher = new ConcurrentBatcher<int>(
    batchSize: 100,
    maxConcurrentBatches: 3
);

await batcher.ProcessBatches(
    items,
    async item => await ProcessItem(item),
    async results => await SaveBatch(results)
);
```

### Task Extensions - LINQ-style

```csharp
using c_Tasking.Extensions;

var result = await Task.FromResult(42)
    .OnSuccess(value => Console.WriteLine($"Success: {value}"))
    .Map(x => x * 2)
    .MapAsync(async x => await FetchData(x));

// Chain operations
await task1
    .Chain(async _ => await task2)
    .Chain(async _ => await task3);

// Handle errors gracefully
await riskyOperation.IgnoreException();
```

## 📚 Core Components

### TaskWrapper
Simplest way to execute tasks without threading complexity.

```csharp
// Synchronous code on thread pool
TaskWrapper.Run(() => DoWork());

// Async functions
await TaskWrapper.RunAsync(async () => await DoAsyncWork());

// Parallel execution
TaskWrapper.RunParallel(task1, task2, task3);

// Wait with timeout
TaskWrapper.WaitWithTimeout(task, 5000);
```

### SimpleThread
Direct thread management with built-in cancellation support.

```csharp
var thread = new SimpleThread();

// Start with cancellation token
thread.Start(cancellationToken =>
{
    while (!cancellationToken.IsCancellationRequested)
    {
        DoWork();
    }
});

// Set priority
thread.SetPriority(ThreadPriority.Highest);

// Graceful stop
thread.Stop(timeoutMilliseconds: 5000);

// Query status
Console.WriteLine($"Running: {thread.IsRunning}");
Console.WriteLine($"ThreadId: {thread.GetThreadId()}");
```

### AsyncOperation
Manual async operation tracking for complex scenarios.

```csharp
// Create from async function
var operation = AsyncOperation.Create(async () =>
{
    await Task.Delay(1000);
});

await operation.WaitAsync();

// With result
var resultOp = AsyncOperation<int>.Create(async () =>
{
    return 42;
});

var result = await resultOp.WaitAsync();

// Manual control
var manual = new AsyncOperation();
manual.SetResult();
manual.SetException(new Exception("Failed"));
manual.Cancel();
```

### ManagedThreadPool
Efficient thread pool with automatic task queuing.

```csharp
using var pool = new ManagedThreadPool(
    maxThreads: Environment.ProcessorCount
);

pool.EnqueueTask(() => { /* work */ });
pool.EnqueueAsync(async () => { /* async work */ });

var stats = pool.GetStats();
Console.WriteLine($"Active threads: {stats.ActiveThreads}");
Console.WriteLine($"Queued tasks: {stats.QueuedTasks}");

pool.WaitAll(timeoutMilliseconds: 30000);
pool.StopAll();
```

### TaskScheduler
Schedule tasks with precise timing control.

```csharp
using var scheduler = new TaskScheduler();

// One-time execution
int taskId = scheduler.ScheduleOnce(
    () => Console.WriteLine("One time"),
    delayMilliseconds: 5000
);

// Repeating
scheduler.ScheduleRepeating(
    () => Console.WriteLine("Every 10s"),
    intervalMilliseconds: 10000
);

// With initial delay then repeat
scheduler.ScheduleWithDelay(
    () => Console.WriteLine("After 5s, then every 10s"),
    delayMilliseconds: 5000,
    intervalMilliseconds: 10000
);

// Cancel specific or all
scheduler.Cancel(taskId);
scheduler.CancelAll();
```

### TaskRetry
Resilient operations with exponential backoff.

```csharp
// Async with retries
var data = await TaskRetry.ExecuteWithRetry(
    async () => await FetchFromApi(),
    maxAttempts: 5,
    initialDelayMilliseconds: 100
);

// Sync operations
var result = TaskRetry.Execute(
    () => ParseData(),
    maxAttempts: 3
);

// Custom retry conditions
await TaskRetry.ExecuteWithRetry(
    async () => await Operation(),
    maxAttempts: 3,
    shouldRetry: ex => 
        ex is TimeoutException or 
        ex is HttpRequestException
);
```

### ConcurrentBatcher
Process items in batches with controlled concurrency.

```csharp
var batcher = new ConcurrentBatcher<T>(
    batchSize: 50,
    maxConcurrentBatches: 4
);

// Process with results
await batcher.ProcessBatches(
    items,
    async item => await ProcessAsync(item),
    async results => await SaveAsync(results)
);

// Process without results
await batcher.ProcessBatches(
    items,
    async item => await DoWork(item),
    async () => Console.WriteLine("Batch complete")
);
```

### TaskExtensions
LINQ-style extensions for cleaner async code.

```csharp
// Success handling
await task.OnSuccess(result => 
    Console.WriteLine($"Result: {result}")
);

// Error handling
await task.OnException(ex => 
    Console.WriteLine($"Error: {ex.Message}")
);

// Execute on cancellation
await task.OnCancelled(() => 
    Console.WriteLine("Cancelled")
);

// Always execute
await task.Finally(() => 
    Console.WriteLine("Done")
);

// Transform results
var doubled = await Task.FromResult(5)
    .Map(x => x * 2);

// Async transformation
var data = await Task.FromResult("api")
    .MapAsync(async endpoint => await Fetch(endpoint));

// Chain operations
var result = await task1
    .Chain(async _ => await task2)
    .Chain(async _ => await task3);

// Safe operations
await risky.IgnoreException();
var value = await riskyWithResult.IgnoreException<int>();

// Parallel operations
await TaskExtensions.WaitAllInParallel(t1, t2, t3);
await TaskExtensions.WaitAnyToComplete(t1, t2, t3);
```

## 💡 Common Patterns

### Pattern 1: Resilient API Calls

```csharp
var data = await TaskRetry.ExecuteWithRetry(
    async () => await apiClient.GetDataAsync(),
    maxAttempts: 3,
    initialDelayMilliseconds: 100,
    shouldRetry: ex => ex is HttpRequestException
);
```

### Pattern 2: Background Processing with Scheduler

```csharp
using var scheduler = new TaskScheduler();

scheduler.ScheduleRepeating(async () =>
{
    await ProcessBackgroundJobs();
}, intervalMilliseconds: 60000); // Every minute
```

### Pattern 3: Bulk Data Processing

```csharp
using var pool = new ManagedThreadPool(Environment.ProcessorCount);

foreach (var item in largeDataset)
{
    pool.EnqueueTask(() => ProcessItem(item));
}

pool.WaitAll();
```

### Pattern 4: Batch API Uploads

```csharp
var batcher = new ConcurrentBatcher<DataItem>(batchSize: 100);

await batcher.ProcessBatches(
    dataItems,
    async item => await apiClient.UploadAsync(item),
    async results => Console.WriteLine($"Uploaded {results.Count} items")
);
```

### Pattern 5: Graceful Shutdown

```csharp
var thread = new SimpleThread();
thread.Start(cancellationToken =>
{
    while (!cancellationToken.IsCancellationRequested)
    {
        DoWork();
    }
});

// Graceful cleanup
using (pool) { }
using (scheduler) { }
thread.Stop(timeoutMilliseconds: 10000);
```

## 🔧 Advanced Usage

### Custom Thread Priorities

```csharp
var highPriority = new SimpleThread();
highPriority.SetPriority(ThreadPriority.Highest);
highPriority.Start(() => CriticalWork());

var lowPriority = new SimpleThread();
lowPriority.SetPriority(ThreadPriority.Lowest);
lowPriority.Start(() => BackgroundWork());
```

### Manual Async Operation Control

```csharp
var operation = new AsyncOperation<string>();

_ = Task.Run(async () =>
{
    try
    {
        var result = await FetchDataAsync();
        operation.SetResult(result);
    }
    catch (Exception ex)
    {
        operation.SetException(ex);
    }
});

var data = await operation.WaitAsync(timeoutMilliseconds: 5000);
```

### Thread Pool Statistics

```csharp
using var pool = new ManagedThreadPool(4);

for (int i = 0; i < 100; i++)
{
    pool.EnqueueTask(() => Thread.Sleep(1000));
}

var stats = pool.GetStats();
Console.WriteLine($"Max Threads: {stats.MaxThreads}");
Console.WriteLine($"Active: {stats.ActiveThreads}");
Console.WriteLine($"Queued: {stats.QueuedTasks}");
Console.WriteLine($"Total: {stats.TotalThreads}");
```

## 🎓 Best Practices

1. **Always use `using` for resource management**
   ```csharp
   using var pool = new ManagedThreadPool();
   using var scheduler = new TaskScheduler();
   ```

2. **Set appropriate timeouts**
   ```csharp
   TaskWrapper.WaitWithTimeout(task, 30000); // 30 seconds
   ```

3. **Use cancellation tokens for long operations**
   ```csharp
   thread.Start(cancellationToken =>
   {
       while (!cancellationToken.IsCancellationRequested)
       {
           DoWork();
       }
   });
   ```

4. **Implement retry logic for network operations**
   ```csharp
   await TaskRetry.ExecuteWithRetry(
       async () => await NetworkCall(),
       maxAttempts: 3
   );
   ```

5. **Monitor thread pool statistics**
   ```csharp
   if (pool.GetStats().QueuedTasks > 1000)
   {
       LogWarning("High queue backlog");
   }
   ```

6. **Batch large workloads**
   ```csharp
   await batcher.ProcessBatches(items, processor);
   ```

## 📋 API Reference

### TaskWrapper
- `Run(Action)` - Execute action asynchronously
- `Run<T>(Func<T>)` - Execute function asynchronously
- `RunAsync(Func<Task>)` - Execute async function
- `RunAsync<T>(Func<Task<T>>)` - Execute async function with result
- `RunParallel(params Task[])` - Execute multiple tasks in parallel
- `WaitWithTimeout(Task, int)` - Wait with timeout
- `WaitAny(params Task[])` - Wait for first task

### SimpleThread
- `Start(Action)` - Start thread
- `Start(Action<CancellationToken>)` - Start with cancellation
- `Join(int)` - Wait for thread
- `Stop(int)` - Stop gracefully
- `SetPriority(ThreadPriority)` - Set priority
- `SetAsBackgroundThread(bool)` - Set background flag
- `GetThreadId()` - Get thread ID
- `IsRunning` - Check if running
- `IsAlive` - Check if alive

### ManagedThreadPool
- `EnqueueTask(Action)` - Queue task
- `EnqueueAsync(Func<Task>)` - Queue async task
- `WaitAll(int)` - Wait all complete
- `StopAll(int)` - Stop all threads
- `GetStats()` - Get statistics
- `Dispose()` - Cleanup

### TaskScheduler
- `ScheduleOnce(Action, int)` - Run once
- `ScheduleRepeating(Action, int)` - Run repeatedly
- `ScheduleWithDelay(Action, int, int)` - Run with delay
- `ScheduleOnceAsync(Func<Task>, int)` - Async once
- `ScheduleRepeatingAsync(Func<Task>, int)` - Async repeat
- `Cancel(int)` - Cancel task
- `CancelAll()` - Cancel all

### TaskRetry
- `ExecuteWithRetry<T>(Func<Task<T>>, ...)` - Async retry
- `ExecuteWithRetry(Func<Task>, ...)` - Async retry
- `Execute<T>(Func<T>, ...)` - Sync retry

### ConcurrentBatcher<T>
- `ProcessBatches<TResult>(IEnumerable<T>, ...)` - Process batches
- Constructor: `ConcurrentBatcher(batchSize, maxConcurrentBatches)`

### TaskExtensions
- `OnSuccess(task, callback)` - Run on success
- `OnException(task, callback)` - Run on error
- `OnCancelled(task, callback)` - Run on cancel
- `Finally(task, callback)` - Run always
- `Map<T>(task, mapper)` - Transform
- `MapAsync<T>(task, mapper)` - Async transform
- `Chain(task, next)` - Chain operations
- `TryWait(task, ms)` - Wait with timeout
- `IgnoreException(task)` - Suppress errors

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙌 Support

For issues, questions, or suggestions, please open an issue on GitHub or visit the documentation.

## 📈 Changelog

See [CHANGELOG.md](CHANGELOG.md) for version history and updates.

---

**Made with ❤️ for the .NET community**
