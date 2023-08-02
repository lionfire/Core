using LionFire.Data.Reactive;
using LionFire.Data.Sets;
using System.Reactive.Subjects;

namespace LionFire.Data;

internal interface ISetsInternal<TValue> : Reactive.IValueRx<TValue>
{
    IEqualityComparer<TValue> EqualityComparer { get; }
    ISetOperation<TValue> SetState { get; }
    object setLock { get; }
    BehaviorSubject<ISetOperation<TValue>> sets { get; }
    Task<ITransferResult> SetImpl(TValue? value, CancellationToken cancellationToken = default);
}
