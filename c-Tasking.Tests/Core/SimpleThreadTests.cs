using c_Tasking.Core;
using Xunit;

namespace c_Tasking.Tests.Core;

public class SimpleThreadTests
{
    [Fact]
    public void Start_ExecutesAction()
    {
        bool executed = false;
        var t = new SimpleThread();
        t.Start(() => executed = true);
        t.Join(1000);
        Assert.True(executed);
    }

    [Fact]
    public void Stop_StopsThreadWithCancellation()
    {
        var t = new SimpleThread();
        bool started = false;
        t.Start(ct =>
        {
            started = true;
            while (!ct.IsCancellationRequested)
            {
                Thread.Sleep(10);
            }
        });
        Assert.True(started);
        t.Stop(200);
        Assert.False(t.IsRunning);
    }

    [Fact]
    public void Join_ReturnsTrueAfterComplete()
    {
        var t = new SimpleThread();
        t.Start(() => Thread.Sleep(50));
        var ok = t.Join(1000);
        Assert.True(ok);
    }
}
