using LionFire.Inspection.Nodes;

namespace LionFire.Inspection;

public interface IInspectorService
{
    IEnumerable<IInspector> Inspectors { get; }
    IServiceProvider ServiceProvider { get; }
    InspectorContext Context { get; }

    INode Inspect(object obj, InspectorContext? context = null) => new InspectedNode(obj, context ?? Context);
}
