using LionFire.Ontology;
using System.Collections.Generic;

namespace LionFire.Collections
{
    public interface IHierarchyOfKeyedOnDemand<T> : IHierarchyOfKeyed<T>
    {
        T GetChild(string key);
    }
}
