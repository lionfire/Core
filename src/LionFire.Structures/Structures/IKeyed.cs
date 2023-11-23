#nullable enable

using LionFire.Ontology;

namespace LionFire.Structures;

public interface IKeyed : IKeyed<string> { }

public interface IKeyed<TKey>
{
    // TODO: Make not nullable
    [Immutable]
    TKey Key { get; }
}
