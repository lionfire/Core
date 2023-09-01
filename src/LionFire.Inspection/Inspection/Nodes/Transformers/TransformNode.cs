using LionFire.Data.Async.Gets;

namespace LionFire.Inspection.Nodes;

public interface ITransformNode : INode
{
    IGetter<object> TransformedSource { get; }
}

public abstract class TransformNode : Node<TransformInfo>, ITransformNode
{
    #region Lifecycle

    public TransformNode(INode? parent, object? source, TransformInfo info, string? key = null, InspectorContext? context = null) : base(parent, source, info, key, context)
    {
    }

    #endregion

    public abstract IGetter<object> TransformedSource { get; }

    /// <summary>
    /// Must have a IGetter<object>
    /// </summary>
    public INode? TransformationParent { get; set; }
    //public object? UntransformedSource { get; set; }

    public int TransformationDepth { get; set; }
}
