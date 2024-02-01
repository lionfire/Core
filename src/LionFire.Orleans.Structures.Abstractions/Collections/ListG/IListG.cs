
using LionFire.Orleans_.ObserverGrains;

namespace LionFire.Orleans_.Collections;

/// <summary>
/// (For non-keyed lists.  If list is keyed, use IKeyedListG)
/// </summary>
/// <typeparam name="TItem"></typeparam>
public interface IListG<TItem>
    : IListBaseG<TItem>
    , IGrainObservableAsyncObservableG<ChangeSet<TItem>>
{
}
