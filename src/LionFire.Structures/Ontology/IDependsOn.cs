#nullable enable

namespace LionFire.Ontology;

public interface IDependsOn<T>
{
    T Dependency { set; }
}
