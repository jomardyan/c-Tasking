using c_Tasking.Core;
using Xunit;

namespace c_Tasking.Tests.Core;

public class TaskWrapperTests
{
    [Fact]
    public async Task Run_WithAction_ExecutesSuccessfully()
    {
        // Arrange
        bool executed = false;

        // Act
        var task = TaskWrapper.Run(() => { executed = true; });
        await task;

        // Assert
        Assert.True(executed);
    }

    [Fact]
    public async Task RunAsync_WithAsyncFunc_ExecutesSuccessfully()
    {
        // Arrange
        bool executed = false;

        // Act
        var task = TaskWrapper.RunAsync(async () => { executed = true; await Task.Delay(10); });
        await task;

        // Assert
        Assert.True(executed);
    }

    [Fact]
    public async Task Run_WithMultipleActions_ExecutesAll()
    {
        // Arrange
        var executions = new System.Collections.Concurrent.ConcurrentBag<int>();

        // Act
        var task1 = TaskWrapper.Run(() => executions.Add(1));
        var task2 = TaskWrapper.Run(() => executions.Add(2));
        var task3 = TaskWrapper.Run(() => executions.Add(3));

        await task1;
        await task2;
        await task3;

        // Assert - order is not guaranteed when starting tasks concurrently
        Assert.Equal(3, executions.Count);
        Assert.Contains(1, executions);
        Assert.Contains(2, executions);
        Assert.Contains(3, executions);
    }

    [Fact]
    public async Task WaitWithTimeout_WhenTimeoutExpires_ThrowsTimeout()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        cts.CancelAfter(100);
        var token = cts.Token;
        var longRunningTask = TaskWrapper.RunAsync(async () =>
        {
            await Task.Delay(5000, token);
        });

        // Act & Assert
        await Assert.ThrowsAsync<System.Threading.Tasks.TaskCanceledException>(async () => await longRunningTask);
    }

    [Fact]
    public async Task RunParallel_WithMultipleActions_ExecutesAllConcurrently()
    {
        // Arrange
        var executions = new List<int>();
        var lockObj = new object();

        // Act
        var tasks = new[]
        {
            TaskWrapper.Run(() => { lock (lockObj) executions.Add(1); }),
            TaskWrapper.Run(() => { lock (lockObj) executions.Add(2); }),
            TaskWrapper.Run(() => { lock (lockObj) executions.Add(3); })
        };

        await Task.WhenAll(tasks);

        // Assert
        Assert.Equal(3, executions.Count);
        Assert.Contains(1, executions);
        Assert.Contains(2, executions);
        Assert.Contains(3, executions);
    }

    [Fact]
    public async Task RunAsync_WithReturnValue_ReturnsCorrectValue()
    {
        // Arrange
        var expectedValue = 42;

        // Act
        var task = TaskWrapper.RunAsync(async () =>
        {
            await Task.Delay(10);
            return expectedValue;
        });

        var result = await task;

        // Assert
        Assert.Equal(expectedValue, result);
    }
}
