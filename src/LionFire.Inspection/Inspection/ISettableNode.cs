using LionFire.Data.Async.Sets;

namespace LionFire.Inspection.Nodes;

public interface ISettableNode
{
    ISetter<object> Setter { get; }
}
