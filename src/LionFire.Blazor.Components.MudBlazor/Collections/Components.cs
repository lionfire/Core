#nullable enable

using LionFire.FlexObjects;

namespace LionFire.Blazor.Components;

public class Components : FlexObject, IComponentized // REVIEW - eliminate this and use Flex interface?
{
    public T TryGetComponent<T>() where T : class
    {
        return this.Query<T>();
    }
    public T GetOrCreateComponent<T>() where T : class
    {
        return this.GetOrCreate<T>();
    }
}

