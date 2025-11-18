# c-Tasking — Usage Cases & Comparison with .NET Built-in APIs

This document compares common usage scenarios using the native .NET primitives versus the c-Tasking library wrappers. It highlights how c-Tasking simplifies certain patterns and when it's appropriate to prefer the library or built-in APIs.

## Goals
- Show simple examples for common patterns: background work, scheduled tasks, retries, batching, and thread management.
- Explain when to use the standard .NET approach versus the c-Tasking helper classes.
- Provide trade-offs on clarity, performance, and control.

---

## Component Mapping

- TaskWrapper ⇄ Task / Task.Run / Task.WhenAll / Task.WhenAny
- SimpleThread ⇄ Thread (System.Threading) / long-running background threads
- AsyncOperation ⇄ TaskCompletionSource / manual Task-based coordination
- ManagedThreadPool ⇄ ThreadPool / TPL (Task Parallel Library)
- TaskScheduler ⇄ System.Threading.Timer / Task.Delay / scheduled jobs (Quartz.NET, Hangfire)
- TaskRetry ⇄ Manual try/catch with Task.Delay or libraries like Polly
- ConcurrentBatcher ⇄ Parallel.ForEach / custom batching using SemaphoreSlim / Dataflow library
- TaskExtensions ⇄ Common Task chaining and continuation patterns

---

## Usage Examples & Comparison

### 1) Simple Asynchronous Task / Fire-and-Forget

.NET (built-in):

```csharp
// Fire-and-forget (use sparingly; exceptions must be handled manually)
_ = Task.Run(async () => {
    try { await DoWorkAsync(); }
    catch (Exception ex) { Log(ex); }
});
```

c-Tasking:

```csharp
// Slightly more concise with TaskWrapper
_ = TaskWrapper.RunAsync(async () => await DoWorkAsync());
```

Why use c-Tasking?
- Cleaner, more intention-revealing syntax for simple one-off tasks.
- For advanced needs or fine-grained control, prefer Task.Run or direct Tasks.

---

### 2) Long-Running/Background Thread

.NET (built-in):

```csharp
var t = new Thread(() => {
    while (!cancellationToken.IsCancellationRequested) {
        DoWork();
    }
});
t.IsBackground = true;
t.Start();
```

c-Tasking:

```csharp
var thread = new SimpleThread();
thread.Start(ct => {
    while (!ct.IsCancellationRequested) {
        DoWork();
    }
});
// Stop gracefully
thread.Stop(5000);
```

Why use c-Tasking?
- `SimpleThread` combines cancellation and lifecycle management, avoiding repeated setup boilerplate.
- Use raw `Thread` for specialized OS thread-configurations or extreme performance tuning.

---

### 3) Managed Thread Pool / Worker Pool

.NET (built-in):
- `ThreadPool` (QueueUserWorkItem) for lightweight tasks.
- TPL for tasks via `Task.Run`, `Parallel.ForEach`, `Dataflow` for pipelines.

c-Tasking:

```csharp
using var pool = new ManagedThreadPool(maxThreads: 4);
pool.EnqueueTask(() => ProcessItem(item));
pool.WaitAll();
```

Why use c-Tasking?
- Simpler interface for queuing tasks where you prefer a bounded worker pool.
- When using higher-level TPL constructs or Dataflow, those may be more appropriate for high scalability or built-in scheduling.

---

### 4) Scheduled Work

.NET (built-in):
- `System.Threading.Timer` or `Task.Delay` loops; for production, Quartz.NET or Hangfire.

c-Tasking:

```csharp
using var scheduler = new TaskScheduler();
int id = scheduler.ScheduleRepeating(() => DoPeriodicWork(), intervalMilliseconds: 10000);
scheduler.Cancel(id);
```

Why use c-Tasking?
- Simplifies repeating + once-off scheduling with built-in cancellation support.
- For enterprise schedulers with persistence or distributed job handling, use third-party libraries.

---

### 5) Retry Logic / Exponential Backoff

.NET (built-in):

```csharp
// Manual approach
for (int i = 0; i < maxAttempts; i++) {
    try {
        return await Operation();
    } catch (Exception ex) {
        if (i == maxAttempts - 1) throw;
        await Task.Delay(100 * (1 << i));
    }
}
```

Polly (recommended in many apps):
```csharp
var policy = Policy.Handle<Exception>().WaitAndRetryAsync(3, i => TimeSpan.FromMilliseconds(100 * Math.Pow(2, i)));
await policy.ExecuteAsync(() => Operation());
```

c-Tasking:

```csharp
var result = await TaskRetry.ExecuteWithRetry(async () => await Operation(), maxAttempts: 3, initialDelayMilliseconds: 100);
```

Why use c-Tasking?
- `TaskRetry` is a lightweight, library-specific retry helper. For more complex policies (circuit-breaker, fallback), prefer Polly.

---

### 6) Batch Processing

.NET (built-in):
- `Parallel.ForEach` or custom batching using `SemaphoreSlim` to limit concurrency.
- `Dataflow` Block pattern for pipeline parallelism.

c-Tasking:

```csharp
var batcher = new ConcurrentBatcher<int>(batchSize: 50, maxConcurrentBatches: 3);
await batcher.ProcessBatches(items, async item => await Process(item), async results => await SaveBatch(results));
```

Why use c-Tasking?
- `ConcurrentBatcher` is built-in and simple to configure for many typical batching needs.
- For more advanced backpressure and memory efficiency, TPL Dataflow or custom streaming design provides more control.

---

## Performance & Behavior Considerations

- c-Tasking intentionally reduces ceremony and boilerplate for common patterns. That usually improves readability and reduces bugs.
- Where performance matters (high-throughput scenarios), prefer native TPL constructs (`Task.Run`, `Parallel`, `ThreadPool`, Dataflow) which are tuned in .NET.
- c-Tasking's thread-pool is useful for bounded concurrency and simpler lifecycle; for high concurrency, prefer `ThreadPool` or `Task` based concurrency.
- Always measure for critical paths: `Stopwatch`, logging counters and load testing to confirm whether the abstraction suits your workload.

---

## Reliability, Cancellation & Disposal

- c-Tasking includes `Dispose` and `Stop` patterns to help with graceful shutdown, and uses `CancellationToken` across APIs by default where it makes sense.
- Native APIs provide more low-level control — you must manage `CancellationTokenSource` and joining threads yourself, and ensure `Dispose()` patterns are enforced for resources.

---

## When to use .NET primitives

- Need the highest performance or scalability.
- Want minimal runtime footprint and direct control.
- Using advanced parallelism patterns requiring TPL Dataflow or `Parallel` constructs.
- Integrating with enterprise scheduler or persistent job systems.

## When to use c-Tasking

- You want simpler, intention-focused APIs for common concurrency scenarios.
- Your codebase can benefit from small, lightweight wrappers making common patterns clearer.
- You prefer a built-in solution that includes retries, scheduling, and throttling helpers without additional dependencies.

---

## Migration tips
- Replace early ad-hoc `Thread` usage with `SimpleThread` when you need cancellation and lifecycle features.
- For modules with `Task.Run` usage, `TaskWrapper` gives simple helper methods — keep `Task.Run` if you need more advanced control.
- If you already use Polly or other libraries for retry and resilience, it is fine to keep them; `TaskRetry` is handy for small projects.

---

## Summary
- c-Tasking is a small, focused set of helpers aimed at making developer intent clearer and code cleaner for common concurrency needs.
- Use it where productivity and simplicity matter. For heavy workloads or enterprise-level control, prefer the underlying .NET primitives or ecosystem tools.

---

## Additional Resources
- `docs/QUICK_REFERENCE.md` — quick examples
- `docs/API_REFERENCE.md` — the consolidated API reference
- `Examples/UsageExamples.cs` — runnable examples from the repo

---

*This document was generated from code and design patterns in the c-Tasking repository.*
