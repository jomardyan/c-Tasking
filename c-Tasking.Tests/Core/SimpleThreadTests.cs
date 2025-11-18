using c_Tasking.Core;
using Xunit;
using System.Threading;
using System.Diagnostics;

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

        // Wait for the started flag to be set by the thread, with a timeout to avoid flakiness
        var sw = Stopwatch.StartNew();
        while (!started && sw.ElapsedMilliseconds < 1000)
        {
            Thread.Sleep(1);
        }

        Assert.True(started);
        Console.WriteLine($"Before stop: IsRunning={t.IsRunning}, IsAlive={t.IsAlive}");
        t.Stop(200);
        Console.WriteLine($"After stop: IsRunning={t.IsRunning}, IsAlive={t.IsAlive}");
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
