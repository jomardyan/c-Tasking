using c_Tasking.Utilities;
using Xunit;

namespace c_Tasking.Tests.Utilities;

public class TaskSchedulerTests
{
    [Fact]
    public async Task ScheduleOnce_ExecutesAfterDelay()
    {
        var scheduler = new c_Tasking.Utilities.TaskScheduler();
        var flag = new ManualResetEventSlim(false);
        scheduler.ScheduleOnce(() => flag.Set(), 50);
        await Task.Delay(150);
        Assert.True(flag.IsSet);
        scheduler.Dispose();
    }

    [Fact]
    public async Task ScheduleRepeating_ExecutesMultipleTimes()
    {
        var scheduler = new c_Tasking.Utilities.TaskScheduler();
        int count = 0;
        var id = scheduler.ScheduleRepeating(() => Interlocked.Increment(ref count), 20);
        await Task.Delay(120);
        Assert.True(Volatile.Read(ref count) >= 3);
        scheduler.Cancel(id);
        scheduler.Dispose();
    }

    [Fact]
    public void Cancel_RemovesTimer()
    {
        var scheduler = new c_Tasking.Utilities.TaskScheduler();
        var id = scheduler.ScheduleRepeating(() => { }, 1000);
        var removed = scheduler.Cancel(id);
        Assert.True(removed);
        scheduler.Dispose();
    }
}
