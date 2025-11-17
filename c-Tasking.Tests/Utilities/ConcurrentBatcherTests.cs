using c_Tasking.Utilities;
using System.Collections.Concurrent;
using Xunit;

namespace c_Tasking.Tests.Utilities;

public class ConcurrentBatcherTests
{
    [Fact]
    public async Task ProcessBatches_WithResults_CompletesAndCallsOnBatch()
    {
        var batcher = new ConcurrentBatcher<int>(batchSize: 3, maxConcurrentBatches: 2);
        var items = Enumerable.Range(1, 7);
        var processed = new ConcurrentBag<int>();
        await batcher.ProcessBatches<int>(items, async i => { await Task.Delay(10); processed.Add(i); return i * 2; }, async (List<int> batch) => { await Task.Delay(1); });
        Assert.Equal(7, processed.Count);
    }

    [Fact]
    public async Task ProcessBatches_NoResults_Completes()
    {
        var batcher = new ConcurrentBatcher<int>(batchSize: 2, maxConcurrentBatches: 2);
        var items = Enumerable.Range(1, 5);
        var processed = new ConcurrentBag<int>();
        await batcher.ProcessBatches(items, async i => { await Task.Delay(10); processed.Add(i); }, async () => { await Task.Delay(1); });
        Assert.Equal(5, processed.Count);
    }
}
