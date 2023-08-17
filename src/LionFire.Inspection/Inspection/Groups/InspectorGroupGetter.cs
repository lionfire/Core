using LionFire.Data.Async.Gets;
using LionFire.Data.Collections;
using LionFire.Data.Mvvm;

namespace LionFire.Inspection.Nodes;

//IObservableCache<INode, string>


//public abstract class OneShotInspectorGroupGetter : InspectorGroupGetter
//{
//    //IEnumerable<INode> 
//}


public abstract class InspectorGroupGetter : GetterRxO<IEnumerable<INode>> // TODO: Change to collection, with TValue IObservableList<INode>
{

    #region Identity

    //public abstract string Key { get; }

    public InspectorGroupInfo Info { get; }

    #endregion

    #region Lifecycle

    public InspectorGroupGetter(InspectorGroupInfo info, object source)
    {
        Info = info;
        Source = source;
    }

    #endregion

    #region Transformation

    public InspectorGroupGetter? TransformationParent { get; set; }
    public int TransformationDepth { get; set; }

    public object? UntransformedSource { get; set; }

    #endregion 

    public object Source { get; set; }
    
}
