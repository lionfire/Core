
using LionFire.Collections;

namespace LionFire.UI
{
    //public interface IUICollection : IUICollection<IUIKeyed> { }

    // FUTURE: strongly typed collections - rework this inheritance hierarchy and use covariant IReadOnlyDictionary
    public interface IUICollection<TChild> : IUICollection // IUIKeyed, IHierarchyOfKeyed<IUIKeyed>
        where TChild : IUIKeyed
    {
        //void Add(TChild node);
        //bool Remove(string key);
    }

    public interface IUICollection : IUIKeyed, IHierarchyOfKeyed<IUIKeyed>
    {
        void Add(IUIKeyed node);
        bool Remove(string key);
        void RemoveAll();
    }

}
