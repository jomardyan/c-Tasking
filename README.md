# c-Tasking

A lightweight C# library that simplifies threading and multitasking for modern .NET projects.

## Highlights
- Task wrappers and helpers for common concurrency patterns
- Simple thread lifecycle management and cancellation support
- Bounded managed thread pool with auto-queuing
- Scheduling utilities for periodic and one-time tasks
- Retry logic with exponential backoff
- Batch processing with controlled concurrency
- LINQ-style task extensions for fluent async code

## Documentation

See the `docs/` folder for full documentation:
- `docs/HELP.md` — Comprehensive guide and API reference
- `docs/USAGE_AND_COMPARISON.md` — Patterns and comparisons with .NET primitives
- `docs/PROJECT_SETUP.md` — Developer setup and CI/CD
- `docs/NUGET_PUBLISHING.md` — Publishing guide
- `docs/CONTRIBUTING.md` — Contribution guidelines
- `docs/CHANGELOG.md` — Release notes

## Installation

```bash
dotnet add package c-Tasking
```

## Quick Examples

Run a simple async task:

```csharp
using c_Tasking.Core;
var result = await TaskWrapper.RunAsync(async () =>
{
    await Task.Delay(100);
    return 42;
});
```

Use a bounded thread pool:

```csharp
using c_Tasking.Core;
using var pool = new ManagedThreadPool(maxThreads: 4);
pool.EnqueueTask(() => Console.WriteLine("Work"));
pool.WaitAll();
```

Schedule periodic work:

```csharp
using c_Tasking.Utilities;
using var scheduler = new TaskScheduler();
scheduler.ScheduleRepeating(() => 
    Console.WriteLine("Every 10 seconds"), 
    intervalMilliseconds: 10000);
```

For more examples, see `Examples/UsageExamples.cs`.

## Key Features

- **Target framework**: .NET 10.0
- **Zero dependencies**: Pure .NET implementation
- **Minimal overhead**: Thin wrappers around .NET APIs
- **Production-ready**: Full resource management and error handling

## Contributing

See `docs/CONTRIBUTING.md` for contribution guidelines.

## License

MIT License – see `LICENSE` for details.
