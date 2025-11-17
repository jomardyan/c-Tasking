namespace c_Tasking.Utilities;

/// <summary>
/// Batches items and processes them concurrently in chunks.
/// </summary>
public class ConcurrentBatcher<T>
{
    private readonly int _batchSize;
    private readonly int _maxConcurrentBatches;

    public ConcurrentBatcher(int batchSize = 10, int maxConcurrentBatches = 4)
    {
        _batchSize = Math.Max(1, batchSize);
        _maxConcurrentBatches = Math.Max(1, maxConcurrentBatches);
    }

    /// <summary>
    /// Processes items in batches concurrently.
    /// </summary>
    public async Task ProcessBatches<TResult>(
        IEnumerable<T> items,
        Func<T, Task<TResult>> processor,
        Func<List<TResult>, Task>? onBatchComplete = null)
    {
        var itemList = items.ToList();
        var batches = itemList
            .Select((item, index) => new { item, index })
            .GroupBy(x => x.index / _batchSize)
            .Select(g => g.Select(x => x.item).ToList())
            .ToList();

        var semaphore = new SemaphoreSlim(_maxConcurrentBatches);

        var tasks = batches.Select(async batch =>
        {
            await semaphore.WaitAsync();
            try
            {
                var results = new List<TResult>();
                foreach (var item in batch)
                {
                    var result = await processor(item);
                    results.Add(result);
                }

                if (onBatchComplete != null)
                {
                    await onBatchComplete(results);
                }
            }
            finally
            {
                semaphore.Release();
            }
        });

        await Task.WhenAll(tasks);
    }

    /// <summary>
    /// Processes items in batches concurrently without returning results.
    /// </summary>
    public async Task ProcessBatches(
        IEnumerable<T> items,
        Func<T, Task> processor,
        Func<Task>? onBatchComplete = null)
    {
        var itemList = items.ToList();
        var batches = itemList
            .Select((item, index) => new { item, index })
            .GroupBy(x => x.index / _batchSize)
            .Select(g => g.Select(x => x.item).ToList())
            .ToList();

        var semaphore = new SemaphoreSlim(_maxConcurrentBatches);

        var tasks = batches.Select(async batch =>
        {
            await semaphore.WaitAsync();
            try
            {
                foreach (var item in batch)
                {
                    await processor(item);
                }

                if (onBatchComplete != null)
                {
                    await onBatchComplete();
                }
            }
            finally
            {
                semaphore.Release();
            }
        });

        await Task.WhenAll(tasks);
    }
}
