using LionFire.Structures;

namespace LionFire.Data.Async.Collections;

public interface IAsyncReadOnlyCollection<TItem> : IEnumerableAsync<TItem>
{
    Task<int> Count { get; }
}


