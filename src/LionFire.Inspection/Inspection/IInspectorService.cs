using LionFire.Inspection.Nodes;
using System.Reactive.Disposables;

namespace LionFire.Inspection;

public interface IInspectorService
{
    IEnumerable<IInspector> Inspectors { get; }
    IServiceProvider ServiceProvider { get; }
    InspectorContext Context { get; }

    //[Obsolete]
    //InspectedNode Inspect(object obj, InspectorContext? context = null)
    //{
    //    var node = new InspectedNode(obj, context ?? Context);
    //    Attach(node);
    //    return node;
    //}

    
}
