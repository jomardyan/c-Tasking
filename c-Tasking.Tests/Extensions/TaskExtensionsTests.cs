using c_Tasking.Extensions;
using Xunit;

namespace c_Tasking.Tests.Extensions;

public class TaskExtensionsTests
{
    [Fact]
    public async Task OnSuccess_ExecutesCallback_WhenSuccessful()
    {
        var called = false;
        var task = Task.FromResult(5);
        await task.OnSuccess<int>(v => { if (v == 5) called = true; });
        Assert.True(called);
    }

    [Fact]
    public async Task OnException_ExecutesCallback_WhenFaulted()
    {
        Exception? ex = null;
        var task = Task.FromException<int>(new InvalidOperationException("boom"));
        await task.OnException(e => ex = e);
        await Task.Delay(10);
        Assert.NotNull(ex);
    }

    [Fact]
    public void TryWait_ReturnsFalse_OnTimeout()
    {
        var t = Task.Delay(200);
        var res = t.TryWait(10);
        Assert.False(res);
    }

    [Fact]
    public async Task Map_TransformsValue()
    {
        var t = Task.FromResult(2).Map(x => x * 3);
        var res = await t;
        Assert.Equal(6, res);
    }

    [Fact]
    public async Task IgnoreException_DoesNotThrow_ReturnsDefault()
    {
        var t = Task.FromException<int>(new InvalidOperationException());
        var result = await t.IgnoreException();
        Assert.Equal(default(int), result);
    }
}
