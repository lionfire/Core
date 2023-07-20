#nullable enable

using LionFire.FlexObjects;

namespace LionFire.Blazor.Components;

public class Components : FlexObject, IComponentized
{
    public T TryGetComponent<T>() where T : class
    {
        return this.Get<T>();
    }
}

