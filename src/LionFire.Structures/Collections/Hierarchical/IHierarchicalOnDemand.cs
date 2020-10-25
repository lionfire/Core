using LionFire.Ontology;
using System.Collections.Generic;

namespace LionFire.Collections
{
    public interface IHierarchicalOnDemand<T> : IHierarchical<T>, IHas<IReadOnlyDictionary<string, T>>
    {
        T GetChild(string key);
    }
}
