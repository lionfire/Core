using LionFire.Inspection.Nodes;

namespace LionFire.Inspection;


public class InspectorService : IInspectorService
{

    public InspectorService(IServiceProvider serviceProvider, IEnumerable<IInspector> inspectors)
    {
        ServiceProvider = serviceProvider;
        Inspectors = inspectors;
        Context = new InspectorContext(this);
    }

    public InspectorContext Context { get; }
    public IServiceProvider ServiceProvider { get; }
    public IEnumerable<IInspector> Inspectors { get; }

    //public IObservableList<IInspector> ObservableInspectors { get; } // ENH, for super dynamic info environments
}

//public static class InspectorServiceX
//{
//    //public static IEnumerable<IInspector> GetInspectorsFor(this InspectorService objectInspectorService, object obj)
//    //{
//    //    foreach (var inspector in objectInspectorService.Inspectors)
//    //    {
//    //        if (inspector.IsSourceSupported(node.Source))
//    //        {
//    //            yield return inspector;
//    //        }
//    //    }
//    //}
//}