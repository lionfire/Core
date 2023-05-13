using LionFire.Structures;

namespace LionFire.Collections.Async;

public interface IAsyncReadOnlyCollection<TItem> : IEnumerableAsync<TItem>
{
    Task<int> Count { get; }
}


