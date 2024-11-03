#nullable enable

namespace LionFire.Ontology;

/// <seealso cref="IHasSettable{T}"/>
public interface IDependsOn<in T>  // RENAME to IInjected?
{
    T Dependency { set; }
}
