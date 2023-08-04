using LionFire.Data.Async.Sets;
using System.Reactive.Subjects;

namespace LionFire.Data.Async;

internal interface ISetsInternal<TValue> : IValueRxO<TValue>
{
    IEqualityComparer<TValue> EqualityComparer { get; }
    ISetOperation<TValue> SetState { get; }
    object setLock { get; }
    BehaviorSubject<ISetOperation<TValue>> sets { get; }
    //Task<ISetResult<TValue>> SetImpl(TValue? value, CancellationToken cancellationToken = default);
    Task<ISetResult<T>> SetImpl<T>(T? value, CancellationToken cancellationToken = default) where T : TValue;
}
