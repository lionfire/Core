#if UNUSED
using LionFire.Structures;

namespace LionFire.Structures.Collections;

public interface IAsyncReadOnlyCollection<TItem> : IEnumerableAsync<TItem>
{
    Task<int> Count { get; }
}


#endif