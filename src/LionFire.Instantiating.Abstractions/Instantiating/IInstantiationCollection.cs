using System;
using System.Collections.Generic;
using LionFire.Collections;
using LionFire.Ontology;
using LionFire.Referencing;

namespace LionFire.Instantiating
{
    public interface IInstantiationCollection :
        IEnumerable<IInstantiation>,
        //, IParented<InstantiationCollection>
             IParented
#if !AOT
                , INotifyCollectionChanged<IInstantiation>
#endif
    {

        event Action<IInstantiation, int> ChildCountChanged;
        new event NotifyCollectionChangedHandler<IInstantiation> CollectionChanged;

        IEnumerable<IInstantiation> Values { get; }
        int Count { get; }
        void Add(IInstantiation instantiation);
        void Add(RH<ITemplate> hTemplate);
        void Add(ITemplate template);
        string CreateKey(ITemplate template);
        string GetDefaultKey(ITemplate template);
        string GetNextValidKeyIncrementForKey(string key);
        void OnChildCountChanged(IInstantiation child, int oldCount);
        void RaiseAdded(IInstantiation child, int oldCount);
        void RaiseRemoved(IInstantiation child, int oldCount);
    }
}