using LionFire.Data.Async.Gets;
using LionFire.Data.Collections;
using LionFire.Data.Mvvm;

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


public abstract class InspectorGroup : AsyncReadOnlyDictionary<string, INode>
{
    #region Identity

    public abstract InspectorGroupInfo Info { get; }

    public IInspector Inspector { get; set; }

    #endregion

    #region Options

    public GetterOptions? GetOptions { get; set; }

    #endregion

    #region Lifecycle

    public InspectorGroup(IInspector inspector, INode sourceNode, INode node)
    {
        SourceNode = sourceNode;
        Node = node;
        Inspector = inspector;
    }        

    #endregion

    #region Transformation

    public InspectorGroup? TransformationParent { get; set; }
    public int TransformationDepth { get; set; }

    public object? UntransformedSource { get; set; }

    #endregion 

    public INode SourceNode { get; set; }
    public object? Source => SourceNode.Source;

    public INode Node { get; }

    #region State

    public bool Subscribe { get; set; } = true;

    #endregion


}
