using c_Tasking.Core;
using Xunit;

namespace c_Tasking.Tests.Core;

public class ManagedThreadPoolTests
{
    [Fact]
    public void EnqueueTask_ExecutesTask()
    {
        var pool = new ManagedThreadPool(2);
        var flag = new ManualResetEventSlim(false);
        pool.EnqueueTask(() => flag.Set());
        pool.WaitAll(1000);
        Assert.True(flag.IsSet);
        pool.Dispose();
    }

    [Fact]
    public void EnqueueAsync_ExecutesAsyncTask()
    {
        var pool = new ManagedThreadPool(2);
        var flag = new ManualResetEventSlim(false);
        pool.EnqueueAsync(async () => { await Task.Delay(10); flag.Set(); });
        pool.WaitAll(1000);
        Assert.True(flag.IsSet);
        pool.Dispose();
    }

    [Fact]
    public void GetStats_ReturnsNumbers()
    {
        var pool = new ManagedThreadPool(1);
        pool.EnqueueTask(() => Thread.Sleep(10));
        var stats = pool.GetStats();
        Assert.True(stats.MaxThreads >= 1);
        pool.Dispose();
    }
}
