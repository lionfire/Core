using LionFire.Ontology;
using LionFire.Structures;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LionFire.Collections.Async;

/// <typeparam name="TItem"></typeparam>
/// <remarks>For uses that can't handle properties</remarks>
public interface IAsyncCollectionBase<TItem> : IEnumerableAsync<TItem>
{

    Task Add(TItem item);

    Task Clear();

    Task<bool> Contains(TItem item);

    Task CopyTo(TItem[] array, int arrayIndex);

    Task<bool> Remove(TItem item);
}

/// <remarks>
/// Adds properties (which Orleans can't handle) to base interface 
/// </remarks>
public interface IAsyncCollection<TItem> : IAsyncCollectionBase<TItem>
{
    Task<bool> IsReadOnly { get; }

    Task<int> Count { get; } // Eliminated for Orleans

}

