using LionFire.Structures;

namespace LionFire.Collections.Async;

/// <typeparam name="TItem"></typeparam>
/// <remarks>For uses that can't handle properties</remarks>
public interface IAsyncCollectionBase<TItem> : IEnumerableAsync<TItem>
{

    Task Add(TItem item);

    Task Clear();

    Task<bool> Contains(TItem item);

    Task<bool> Remove(TItem item);

    // Not relevant for Async:
    //Task CopyTo(TItem[] array, int arrayIndex);
}

