using System.Collections.Generic;

namespace LionFire.Structures;

/// <summary>
/// An Async Enumerable, but an entire synchronous enumerable is returned asynchronously rather than the approach of System.Collections.Generic.IAsyncEnumerable
/// </summary>
/// <typeparam name="TValue"></typeparam>
public interface IEnumerableAsync<TValue>
{
    Task<IEnumerable<TValue>> GetEnumerableAsync();
}

