namespace c_Tasking.Utilities;

using c_Tasking.Core;

/// <summary>
/// Batches items and processes them concurrently in chunks.
/// </summary>
public class ConcurrentBatcher<T>
{
    private readonly int _batchSize;
    private readonly int _maxConcurrentBatches;

    /// <summary>
    /// Creates a new concurrent batcher with specified batch size and maximum concurrent batches.
    /// </summary>
    public ConcurrentBatcher(int batchSize = 10, int maxConcurrentBatches = 4)
    {
        _batchSize = Math.Max(1, batchSize);
        _maxConcurrentBatches = Math.Max(1, maxConcurrentBatches);
    }

    /// <summary>
    /// Processes items in batches concurrently.
    /// </summary>
    /// <param name="items">Items to process in batches.</param>
    /// <param name="processor">Processor that transforms each item to the result.</param>
    /// <param name="onBatchComplete">Optional callback executed after each batch completes with results.</param>
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
                        try
                        {
                            var result = await processor(item);
                            results.Add(result);
                        }
                        catch (Exception ex)
                        {
                            ErrorHandler.Instance.Log(ex, "ConcurrentBatcher.ProcessBatches(item)");
                            throw;
                        }
                    }

                    if (onBatchComplete != null)
                    {
                        try
                        {
                            await onBatchComplete(results);
                        }
                        catch (Exception ex)
                        {
                            ErrorHandler.Instance.Log(ex, "ConcurrentBatcher.onBatchComplete");
                            throw;
                        }
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
    /// <param name="items">Items to process in batches.</param>
    /// <param name="processor">Processor that processes each item without result.</param>
    /// <param name="onBatchComplete">Optional callback executed after each batch completes.</param>
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
