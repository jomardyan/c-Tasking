namespace c_Tasking;

/// <summary>
/// c-Tasking Library - A comprehensive C# wrapper for .NET threading and multitasking.
/// 
/// This library simplifies complex threading and async operations by providing
/// easy-to-use wrappers around .NET's native threading and Task-based APIs.
/// 
/// Main Components:
/// - TaskWrapper: Simple synchronous and asynchronous task execution
/// - SimpleThread: Easy thread management with cancellation support
/// - AsyncOperation: Manual async operation tracking
/// - ManagedThreadPool: Thread pool for managing multiple concurrent tasks
/// - TaskScheduler: Schedule tasks to run at specific times or intervals
/// - TaskRetry: Automatic retry logic with exponential backoff
/// - ConcurrentBatcher: Process items in batches concurrently
/// - TaskExtensions: LINQ-like extensions for tasks
/// </summary>
public class Library
{
    /// <summary>
    /// Main entry used for library examples and quick local testing.
    /// Not intended for production use when used as a library package.
    /// </summary>
    public static void Main()
    {
        Console.WriteLine("c-Tasking Library - Multi-threading and Tasking Made Simple");
        Console.WriteLine("See UsageExamples.cs for comprehensive usage examples");
    }
}
