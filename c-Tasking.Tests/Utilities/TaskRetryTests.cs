using c_Tasking.Utilities;
using Xunit;

namespace c_Tasking.Tests.Utilities;

public class TaskRetryTests
{
    [Fact]
    public async Task ExecuteWithRetry_ReturnsValueAfterRetries()
    {
        int attempts = 0;
        var result = await TaskRetry.ExecuteWithRetry<int>(async () =>
        {
            attempts++;
            if (attempts < 2) throw new InvalidOperationException();
            await Task.Delay(10);
            return 42;
        }, maxAttempts: 3, initialDelayMilliseconds: 10);

        Assert.Equal(42, result);
        Assert.True(attempts >= 2);
    }

    [Fact]
    public async Task ExecuteWithRetry_ThrowsAfterMaxAttempts()
    {
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await TaskRetry.ExecuteWithRetry(async () => { await Task.Delay(5); throw new InvalidOperationException(); }, maxAttempts: 2, initialDelayMilliseconds: 10)
        );
    }
}
