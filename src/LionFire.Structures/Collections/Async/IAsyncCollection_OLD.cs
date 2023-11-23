
namespace LionFire.Data.Collections;

/// <remarks>
/// Adds properties (which Orleans can't handle) to base interface 
/// </remarks>
public interface IAsyncCollection_OLD<TItem> : IAsyncCollectionBase_OLD<TItem> // TODO: Reconcile with LionFire.Data.Async, which might have more composable / customizable building blocks
{
    Task<bool> IsReadOnly { get; }

    Task<int> Count { get; } // Eliminated for Orleans

}

