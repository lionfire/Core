using MorseCode.ITask;
using LionFire.Data.Async;
using LionFire.Data.Async.Gets;
using LionFire.Data.Collections;
using LionFire.Data.Mvvm;
using LionFire.Structures;
using System.Collections.Generic;
using System.Reactive.Linq;
using LionFire.IO;

namespace LionFire.Inspection.Nodes;

//IObservableCache<INode, string>
//public abstract class OneShotInspectorGroupGetter : InspectorGroupGetter

//internal abstract class XGetter
//    : SynchronousOneShotGetter<IEnumerable<INode>>
//    , IObservableCacheKeyableGetter<string, INode>
//{
//    protected object Source { get; }

//    public XGetter(object source)
//    {
//        Source = source;
//    }
//}

public interface IInspectorGroup : IDictionaryGetter<string, INode>
{
    GroupInfo Info { get; }
}

public abstract class FrozenGroup : IInspectorGroup
{
    public abstract GroupInfo Info { get; }

    #region Value

    public abstract IDictionary<string, INode>? Value { get; }

    IDictionary<string, INode>? IGetter<IDictionary<string, INode>>.ReadCacheValue => Value;

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

    ITask<IGetResult<IDictionary<string, INode>>> IStatelessGetter<IDictionary<string, INode>>.Get(CancellationToken cancellationToken) => Task.FromResult(queryValue()).AsITask();
    IObservable<ITask<IGetResult<IDictionary<string, INode>>>> IObservableGetOperations<IDictionary<string, INode>>.GetOperations => Observable.Return(Task.FromResult(queryValue()).AsITask());

    ITask<IGetResult<IDictionary<string, INode>>> IGetter<IDictionary<string, INode>>.GetIfNeeded() => Task.FromResult(queryValue()).AsITask();

    private IGetResult<IDictionary<string, INode>> queryValue() => GetResult<IDictionary<string, INode>>.NoopSuccess(Value ?? (empty ??= new()));

    IGetResult<IDictionary<string, INode>> IGetter<IDictionary<string, INode>>.QueryValue() => queryValue();

    #region (static)

    private static Dictionary<string, INode> empty; // TODO .NET8 - FrozenDictionary
    
    #endregion

    #endregion

}

