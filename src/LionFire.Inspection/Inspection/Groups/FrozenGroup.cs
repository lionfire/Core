using MorseCode.ITask;
using LionFire.Data.Async;
using LionFire.Data.Async.Gets;
using LionFire.Data.Mvvm;
using LionFire.Structures;
using System.Collections.Generic;
using System.Reactive.Linq;
using LionFire.IO;
using static System.Net.WebRequestMethods;

namespace LionFire.Inspection.Nodes;
//public abstract class OneShotInspectorGroupGetter : InspectorGroupGetter

//internal abstract class XGetter
//    : SynchronousOneShotGetter<IEnumerable<INode>>
//    , IObservableCacheKeyableGetter<string, INode>

public abstract class FrozenGroup : GroupNode, IInspectorGroup
{



    protected FrozenGroup(IInspector inspector, INode? parent, GroupInfo info, string? key = null, InspectorContext? inspectorContext = null) : base(inspector, parent, info, key, inspectorContext)
    {
    }

    #region Value

    //public abstract IDictionary<string, INode>? Value { get; }

    //IDictionary<string, INode>? IGetter<IDictionary<string, INode>>.ReadCacheValue => Value;

    public bool HasValue => true;

    #region Discard

    public void Discard()
    {
        DiscardValue();
    }

    public virtual void DiscardValue()
    {
    }

    #endregion

    #endregion

    #region IGetter

    // OLD
    //ITask<IGetResult<IDictionary<string, INode>>> IStatelessGetter<IDictionary<string, INode>>.Get(CancellationToken cancellationToken) => Task.FromResult(queryValue()).AsITask();
    //IObservable<ITask<IGetResult<IDictionary<string, INode>>>> IObservableGetOperations<IDictionary<string, INode>>.GetOperations => Observable.Return(Task.FromResult(queryValue()).AsITask());
    //ITask<IGetResult<IDictionary<string, INode>>> IGetter<IDictionary<string, INode>>.GetIfNeeded() => Task.FromResult(queryValue()).AsITask();

    //private IGetResult<IDictionary<string, INode>> queryValue() => GetResult<IDictionary<string, INode>>.NoopSuccess(Value ?? (empty ??= new()));

    //IGetResult<IDictionary<string, INode>> IGetter<IDictionary<string, INode>>.QueryValue() => queryValue();

    #region (static)

    //private static Dictionary<string, INode> empty; // TODO .NET8 - FrozenDictionary

    #endregion

    #endregion

}

