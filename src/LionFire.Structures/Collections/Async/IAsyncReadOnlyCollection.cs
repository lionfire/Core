using LionFire.Structures;

namespace LionFire.Data.Collections;

public interface IAsyncReadOnlyCollection<TItem> : IEnumerableAsync<TItem>
{
    Task<int> Count { get; }
}


