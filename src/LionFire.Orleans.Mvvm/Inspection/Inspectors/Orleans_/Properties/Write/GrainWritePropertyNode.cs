using LionFire.Inspection.Nodes;
using LionFire.Data.Async.Sets;

namespace LionFire.Inspection;

public class GrainWritePropertyNode : Node<GrainPropertyInfo>, ISetter<object>
{
    public GrainWritePropertyNode(INode? parent, object? source, GrainPropertyInfo info, string? key = null, InspectorContext? context = null) : base(parent, source, info, key, context)
    {
        Setter = new GrainPropertySetter(this);
    }

    GrainPropertySetter Setter { get; set; }

    #region Pass-thru

    public Task<ISetResult<object>> Set(object? value, CancellationToken cancellationToken = default)
    {
        return Setter.Set(value, cancellationToken);
    }

    #endregion

}
