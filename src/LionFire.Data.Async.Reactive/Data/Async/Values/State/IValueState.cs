using LionFire.Data.Async.Sets;
using LionFire.IO;
using LionFire.Reactive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Data.Async;

/// <summary>
/// Common interface for IGetter, IAsyncValue, ISetter: some interfaces will throw NotSupportedException.
/// </summary>
public interface IValueState : IIOCapable, IWriteStagesSet, IReactiveObjectEx
{
    ValueStateFlags StateFlags { get; }

    // REVIEW: is overwriting interfaces like this going to work?
    new Task<ISetResult> Set(CancellationToken cancellationToken = default) => throw new NotSupportedException();
    new void DiscardStagedValue() => throw new NotSupportedException();
}

/// <summary>
/// Common interface for IGetter, IAsyncValue, ISetter: some interfaces will throw NotSupportedException.
/// </summary>
/// <remarks>
/// Not supported by IStatelessGetter
/// </remarks>
public interface IValueState<TValue> : IValueState
{
    TValue? Value
    {
        // IGetter (lazy): ReadCacheValue
        // IAsyncValue: StagedValue, if set, otherwise ReadCacheValue
        // ISetter: StagedValue, if set, otherwise null
        get;

        // IGetter: NotSupportedException (Check ValueStateFlags first)
        // IAsyncValue: sets StagedValue
        // ISetter: sets StagedValue
        set;
    }
}

