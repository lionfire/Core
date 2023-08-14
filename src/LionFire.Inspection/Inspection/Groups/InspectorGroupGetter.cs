using LionFire.Data.Async.Gets;

namespace LionFire.Inspection;

public abstract class InspectorGroupGetter : GetterRxO<IEnumerable<IInspectorNode>> // TODO: Change to collection, with TValue IObservableList<IInspectorNode>
{
    #region Identity

    public string Key { get; }

    #endregion

    #region Lifecycle

    public InspectorGroupGetter(string key, object source)
    {
        Key = key;
        Source = source;
    }

    #endregion

    public InspectorGroupGetter? TransformationParent { get; set; }
    public int TransformationDepth { get; set; }

    public object? UntransformedSource { get; set; }

    public object Source { get; set; }


    public InspectorGroupInfo Info { get; set; }



}
