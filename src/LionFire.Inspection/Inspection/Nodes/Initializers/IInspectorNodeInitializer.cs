using LionFire.Hosting;
using LionFire.Inspection.Nodes;
using Microsoft.Extensions.DependencyInjection;

namespace LionFire.Inspection;

public interface IInspectorNodeInitializer
{
    void Init(INode node);
}

public class OrleansInspectorNodeInitializer : IInspectorNodeInitializer
{
    public void Init(INode node)
    {
        if (node is PropertyNode propertyNode)
        {
            if (propertyNode.Info.PropertyInfo.DeclaringType?.FullName == "Orleans.Runtime.GrainReference")
            {
                node.Visibility |= InspectorVisibility.Hidden;
            }
        }
    }
}
public static class OrleansInspectorHostingX
{
    public static IServiceCollection AddOrleansInspectors(this IServiceCollection services) 
        => services.TryAddEnumerableSingleton<IInspectorNodeInitializer, OrleansInspectorNodeInitializer>();
}
