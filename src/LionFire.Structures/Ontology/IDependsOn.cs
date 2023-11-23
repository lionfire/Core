#nullable enable

namespace LionFire.Ontology;

/// <seealso cref="IHasSettable{T}"/>
public interface IDependsOn<T>
{
    T Dependency { set; }
}
