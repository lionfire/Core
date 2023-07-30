using System.Reflection;

namespace LionFire.UI.Components.PropertyGrid;

public static class PropertyInfoX
{
    public static bool IsObservable(this PropertyInfo pi) => pi.PropertyType.IsGenericType && pi.PropertyType.GetGenericTypeDefinition() == typeof(IObservable<>);
    public static bool IsAsyncEnumerable(this PropertyInfo pi) => pi.PropertyType.IsGenericType && pi.PropertyType.GetGenericTypeDefinition() == typeof(IAsyncEnumerable<>);
}
