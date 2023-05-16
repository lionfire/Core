
namespace LionFire.Collections.Async;

/// <remarks>
/// Adds properties (which Orleans can't handle) to base interface 
/// </remarks>
public interface IAsyncCollection<TItem> : IAsyncCollectionBase<TItem>
{
    Task<bool> IsReadOnly { get; }

    Task<int> Count { get; } // Eliminated for Orleans

}

