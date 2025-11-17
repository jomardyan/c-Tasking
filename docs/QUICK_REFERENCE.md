# API Quick Reference

For a consolidated, master reference of the public API, see [API_REFERENCE.md](API_REFERENCE.md).

## Quick Start

```csharp
using c_Tasking.Core;
using c_Tasking.Utilities;
using c_Tasking.Extensions;

// Simple task
await TaskWrapper.RunAsync(async () => { /* work */ });

// Simple thread
new SimpleThread().Start(() => { /* work */ });

// Thread pool
using var pool = new ManagedThreadPool();
pool.EnqueueTask(() => { /* work */ });

// Schedule task
using var scheduler = new TaskScheduler();
scheduler.ScheduleOnce(() => { /* work */ }, 5000);

// Retry operation
await TaskRetry.ExecuteWithRetry(async () => { /* work */ }, maxAttempts: 3);

// Batch process
var batcher = new ConcurrentBatcher<T>(batchSize: 10);
await batcher.ProcessBatches(items, processor);
```

## Core Classes

### TaskWrapper
- `Run(Action)` - Async execution of sync code
- `Run<T>(Func<T>)` - Async execution with return
- `RunAsync(Func<Task>)` - Async execution
- `RunAsync<T>(Func<Task<T>>)` - Async execution with return
- `RunParallel(params Task[])` - Parallel execution
- `WaitWithTimeout(Task, ms)` - Wait with timeout

### SimpleThread
- `Start(Action)` - Start thread
- `Start(Action<CancellationToken>)` - Start with cancellation
- `Join(ms)` - Wait for thread
- `Stop(ms)` - Stop gracefully
- `IsRunning` - Check if running
- `IsAlive` - Check if alive

### AsyncOperation
- `Create(Func<Task>)` - Create operation
- `Create<T>(Func<Task<T>>)` - Create operation with result
- `SetResult()` / `SetResult<T>(T)` - Mark complete
- `SetException(ex)` - Mark failed
- `Cancel()` - Cancel operation
- `WaitAsync()` - Wait for completion

### ManagedThreadPool
- `EnqueueTask(Action)` - Queue task
- `EnqueueAsync(Func<Task>)` - Queue async
- `WaitAll(ms)` - Wait all complete
- `StopAll(ms)` - Stop all threads
- `GetStats()` - Pool statistics
- `Dispose()` - Cleanup

### TaskScheduler
- `ScheduleOnce(Action, delayMs)` - Run once
- `ScheduleRepeating(Action, intervalMs)` - Run repeatedly
- `ScheduleWithDelay(Action, delayMs, intervalMs)` - Run with delay then repeat
- `ScheduleOnceAsync(Func<Task>, delayMs)` - Async once
- `ScheduleRepeatingAsync(Func<Task>, intervalMs)` - Async repeat
- `Cancel(id)` - Cancel specific task
- `CancelAll()` - Cancel all tasks

### TaskRetry
- `ExecuteWithRetry<T>(Func<Task<T>>, maxAttempts, delayMs)` - Retry async
- `ExecuteWithRetry<T>(Func<Task<T>>, maxAttempts, delayMs, shouldRetry)` - Retry with filter
- `ExecuteWithRetry(Func<Task>, ...)` - Retry without result
- `Execute<T>(Func<T>, ...)` - Retry sync

### ConcurrentBatcher<T>
- `ProcessBatches(items, processor)` - Process batches
- `ProcessBatches(items, processor, onComplete)` - Process with callback
- Constructor: `ConcurrentBatcher(batchSize, maxConcurrentBatches)`

### TaskExtensions
- `OnSuccess(task, callback)` - Run on success
- `OnException(task, callback)` - Run on error
- `OnCancelled(task, callback)` - Run on cancel
- `Finally(task, callback)` - Run always
- `Map<T>(task, mapper)` - Transform result
- `Chain(task, nextTask)` - Chain operations
- `TryWait(task, ms)` - Wait with timeout
- `IgnoreException(task)` - Suppress errors

## Namespaces

- `c_Tasking.Core` - TaskWrapper, SimpleThread, AsyncOperation, ManagedThreadPool
- `c_Tasking.Utilities` - TaskScheduler, TaskRetry, ConcurrentBatcher
- `c_Tasking.Extensions` - TaskExtensions
- `c_Tasking.Examples` - UsageExamples

## Common Patterns

### Fire and Forget
```csharp
_ = Task.Run(() => DoWork()).IgnoreException();
```

### Parallel Work
```csharp
await TaskExtensions.WaitAllInParallel(task1, task2, task3);
```

### Sequential Chain
```csharp
await task1.Chain(async _ => await task2);
```

### Conditional Retry
```csharp
await TaskRetry.ExecuteWithRetry(
    operation,
    shouldRetry: ex => ex is TimeoutException
);
```

### Batch and Schedule
```csharp
using var scheduler = new TaskScheduler();
scheduler.ScheduleRepeating(async () =>
{
    await batcher.ProcessBatches(...);
}, intervalMs: 60000);
```
