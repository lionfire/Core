using LionFire.Ontology;
using System.Collections.Generic;

namespace LionFire.Collections
{
    public interface IHierarchical<T> : IHas<IReadOnlyDictionary<string, T>>
    {
    }
}
