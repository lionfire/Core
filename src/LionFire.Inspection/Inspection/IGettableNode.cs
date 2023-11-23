using LionFire.Data.Async.Gets;

namespace LionFire.Inspection.Nodes;

public interface IGettableNode
{
    IGetter<object> Getter { get; }
}
