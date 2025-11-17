# c-Tasking API Reference

This document provides a consolidated, easy-to-scan view of the public API for the c-Tasking library.

Namespaces:
- `c_Tasking.Core`
- `c_Tasking.Utilities`
- `c_Tasking.Extensions`
- `c_Tasking.Examples` (non-library; usage examples)

---

## Core

### AsyncOperation
A lightweight manual async operation monitor for tracking completion without tying up callers.

- `AsyncOperation Create(Func<Task> asyncFunc)`
  - Create and run an async operation backed with `TaskCompletionSource`.
- `Task WaitAsync()`
- `Task WaitAsync(int timeoutMilliseconds)`
- `void SetResult()`
- `void SetException(Exception ex)`
- `void Cancel()`

### AsyncOperation<T>
Generic version of `AsyncOperation` that captures a result.

- `AsyncOperation<T> Create(Func<Task<T>> asyncFunc)`
- `Task<T?> WaitAsync()`
- `Task<T?> WaitAsync(int timeoutMilliseconds)`
- `void SetResult(T? result)`
- `void SetException(Exception ex)`
- `void Cancel()`
- `bool IsCompleted { get; }`
- `T? Result { get; }`

### TaskWrapper
Convenience methods for offloading actions to the `Task` scheduler.

- `Task Run(Action action)`
- `Task<T> Run<T>(Func<T> function)`
- `Task RunAsync(Func<Task> asyncAction)`
- `Task<T> RunAsync<T>(Func<Task<T>> asyncFunction)`
- `void WaitWithTimeout(Task task, int timeoutMilliseconds)`
- `T WaitWithTimeout<T>(Task<T> task, int timeoutMilliseconds)`
- `void RunParallel(params Task[] tasks)`
- `T[] RunParallel<T>(params Task<T>[] tasks)`
- `Task WaitAny(params Task[] tasks)`
- `Task<Task<T>> WaitAny<T>(params Task<T>[] tasks)`

### SimpleThread
A small wrapper simplifying `Thread` lifecycle and cancellation.

- Constructor `SimpleThread()`
- `void Start(Action action)`
- `void Start(Action<CancellationToken> action)`
- `bool Join(int timeoutMilliseconds = Timeout.Infinite)`
- `void Stop(int timeoutMilliseconds = 5000)`
- `void SetPriority(ThreadPriority priority)`
- `void SetAsBackgroundThread(bool isBackground)`
- `int? GetThreadId()`
- `bool IsRunning { get; }`
- `bool IsAlive { get; }`

### ManagedThreadPool
A managed pool for scheduling `Action`-based work across `SimpleThread` instances.

- `ManagedThreadPool(int? maxThreads = null)`
- `void EnqueueTask(Action task)`
- `void EnqueueAsync(Func<Task> asyncTask)`
- `void WaitAll(int timeoutMilliseconds = Timeout.Infinite)`
- `void StopAll(int timeoutMilliseconds = 5000)`
- `ThreadPoolStats GetStats()`
- `int ActiveThreadCount { get; }`
- `int QueuedTaskCount { get; }`
- `void Dispose()`

#### ThreadPoolStats
- `int MaxThreads { get; set; }`
- `int ActiveThreads { get; set; }`
- `int QueuedTasks { get; set; }`
- `int TotalThreads { get; set; }`
- `override string ToString()`

---

## Utilities

### TaskRetry
Retry utilities that implement exponential backoff and optional custom retry predicate.

- `Task<T> ExecuteWithRetry<T>(Func<Task<T>> task, int maxAttempts = 3, int initialDelayMilliseconds = 100)`
- `Task<T> ExecuteWithRetry<T>(Func<Task<T>> task, int maxAttempts, int initialDelayMilliseconds, Func<Exception, bool>? shouldRetry)`
- `Task ExecuteWithRetry(Func<Task> task, int maxAttempts = 3, int initialDelayMilliseconds = 100)`
- `Task ExecuteWithRetry(Func<Task> task, int maxAttempts, int initialDelayMilliseconds, Func<Exception, bool>? shouldRetry)`
- `T Execute<T>(Func<T> task, int maxAttempts = 3, int initialDelayMilliseconds = 100)`
- `T Execute<T>(Func<T> task, int maxAttempts, int initialDelayMilliseconds, Func<Exception, bool>? shouldRetry)`

### AdvancedUtilities
Includes several helpers for parallel execution, timeouts and throttling.

- `ParallelTaskExecutor.ExecuteParallelAsync<T>(IEnumerable<T> items, Func<T, Task> asyncAction, int maxDegreeOfParallelism = 4)`
- `ParallelTaskExecutor.ExecuteParallelAsync<TResult>(IEnumerable<Func<Task<TResult>>> asyncFunctions, int maxDegreeOfParallelism = 4)`

#### TimeoutManager
- `Task<T> ExecuteWithDeadlineAsync<T>(Func<CancellationToken, Task<T>> operation, TimeSpan timeout)`
- `Task ExecuteWithDeadlineAsync(Func<CancellationToken, Task> operation, TimeSpan timeout)`
- `Task<bool> PollUntilAsync(Func<Task<bool>> condition, TimeSpan timeout, TimeSpan pollInterval)`

#### TaskThrottler
- `TaskThrottler(int maxOperations, TimeSpan timeWindow)`
- `Task<T> ThrottleAsync<T>(Func<Task<T>> operation)`
- `Task ThrottleAsync(Func<Task> operation)`
- `void Dispose()`

#### TaskWaiter
- `Task<bool> WaitAllWithTimeoutAsync(IEnumerable<Task> tasks, TimeSpan timeout)`
- `Task<Task> WaitForAnyCompletionAsync(params Task[] tasks)`
- `Task WithCompletionCallback(this Task task, Action onComplete)`

#### ExecutionMetrics
- `long CompletedOperations { get; set; }`
- `long FailedOperations { get; set; }`
- `long CancelledOperations { get; set; }`
- `TimeSpan TotalExecutionTime { get; set; }`
- `double AverageExecutionTime { get; set; }`
- `long PeakConcurrentOperations { get; set; }`

### ConcurrentBatcher<T>
Batching helper to process items in parallel across batches.

- `ConcurrentBatcher(int batchSize = 10, int maxConcurrentBatches = 4)`
- `Task ProcessBatches<TResult>(IEnumerable<T> items, Func<T, Task<TResult>> processor, Func<List<TResult>, Task>? onBatchComplete = null)`
- `Task ProcessBatches(IEnumerable<T> items, Func<T, Task> processor, Func<Task>? onBatchComplete = null)`

### TaskScheduler
Schedule one-time and repeating tasks.

- `TaskScheduler()`
- `int ScheduleOnce(Action task, int delayMilliseconds)`
- `int ScheduleRepeating(Action task, int intervalMilliseconds)`
- `int ScheduleWithDelay(Action task, int delayMilliseconds, int intervalMilliseconds)`
- `int ScheduleOnceAsync(Func<Task> task, int delayMilliseconds)`
- `int ScheduleRepeatingAsync(Func<Task> task, int intervalMilliseconds)`
- `bool Cancel(int taskId)`
- `void CancelAll()`
- `void Dispose()`

---

## Extensions

### TaskExtensions
LINQ-like helpers for `Task` transformations and event callbacks.

- `Task Finally(this Task task, Action callback)`
- `Task OnSuccess(this Task task, Action callback)`
- `Task OnSuccess<T>(this Task<T> task, Action<T> callback)`
- `Task OnException(this Task task, Action<Exception> callback)`
- `Task OnCancelled(this Task task, Action callback)`
- `bool TryWait(this Task task, int timeoutMilliseconds)`
- `(bool completed, T? result) TryWait<T>(this Task<T> task, int timeoutMilliseconds)`
- `Task<TResult> Map<T, TResult>(this Task<T> task, Func<T, TResult> mapper)`
- `Task<TResult> MapAsync<T, TResult>(this Task<T> task, Func<T, Task<TResult>> mapper)`
- `Task Chain(this Task task, Func<Task> nextTask)`
- `Task<TResult> Chain<T, TResult>(this Task<T> task, Func<T, Task<TResult>> nextTask)`
- `Task IgnoreException(this Task task)`
- `Task<T?> IgnoreException<T>(this Task<T> task)`
- `Task WaitAllInParallel(params Task[] tasks)`
- `Task WaitAnyToComplete(params Task[] tasks)`

---

## Examples
The `Examples/UsageExamples.cs` file includes runnable examples for most APIs. The examples are part of the repository for quick local testing and learning.

---

## Notes & Next Steps
- This API reference provides a high-level summary intended for quick use by consumers and in NuGet package descriptions. For method parameter and return details, consult function signatures or IntelliSense, or consider generating API docs using the XML documentation file and a tool such as DocFX.
- Let me know if you want `API_REFERENCE.md` expanded with `param` and `returns` details for every method, or converted into a specialized Markdown layout for the package docs.


*Generated from code and XML documentation comments in the repository.*
