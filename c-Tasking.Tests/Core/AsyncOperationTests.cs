using c_Tasking.Core;
using Xunit;

namespace c_Tasking.Tests.Core;

public class AsyncOperationTests
{
    [Fact]
    public async Task Create_WaitAsync_Completes()
    {
        var op = AsyncOperation.Create(async () => await Task.Delay(10));
        await op.WaitAsync();
        // Allow flag to be set across threads
        await Task.Delay(1);
        Assert.True(op.IsCompleted);
    }

    [Fact]
    public async Task CreateGeneric_ReturnsCorrectValue()
    {
        var op = AsyncOperation.Create(async () => { await Task.Delay(10); return 5; });
        var res = await op.WaitAsync();
        Assert.Equal(5, res);
    }

    [Fact]
    public async Task SetException_MarksCompleted()
    {
        var op = new AsyncOperation();
        op.SetException(new InvalidOperationException());
        await Assert.ThrowsAsync<InvalidOperationException>(async () => await op.WaitAsync(100));
    }
}
