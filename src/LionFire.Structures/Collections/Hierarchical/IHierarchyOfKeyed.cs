using LionFire.Ontology;
using System.Collections.Generic;

namespace LionFire.Collections
{
    public interface IHierarchyOfKeyed<T> // RENAME - IKeyedCollection or INameValueCollection
    {
        IReadOnlyDictionary<string, T> Children { get; }
    }
}
