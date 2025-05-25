#nullable enable

using LionFire.FlexObjects;

namespace LionFire.Blazor.Components;

public class ComponentsContainer : FlexObject, IComponentized // REVIEW - eliminate this and use Flex interface?
{
    public T? TryGetComponent<T>() where T : class => this.Query<T>();
    public T GetOrCreateComponent<T>() where T : class => this.GetOrCreate<T>();
}

