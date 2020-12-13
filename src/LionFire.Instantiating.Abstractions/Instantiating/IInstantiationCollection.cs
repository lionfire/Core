using System;
using System.Collections.Generic;
using LionFire.Collections;
using LionFire.Ontology;
using LionFire.Persistence;
using LionFire.Referencing;

namespace LionFire.Instantiating
{

//    public interface IInstantiationCollection<TTemplate> :
//        IEnumerable<IInstantiation<TTemplate>>,
//             //, IParentable<InstantiationCollection>
//             IParentable
//#if !AOT
//                , INotifyCollectionChanged<IInstantiation<TTemplate>>
//#endif
//        where TTemplate : ITemplate
//    {

//        event Action<IInstantiation<TTemplate>, int> ChildCountChanged;
//        new event NotifyCollectionChangedHandler<IInstantiation<TTemplate>> CollectionChanged;

//        IEnumerable<IInstantiation<TTemplate>> Values { get; }
//        int Count { get; }
//        void Add(IInstantiation<TTemplate> instantiation);
//        void Add(IReadHandleBase<TTemplate> hTemplate);
//        void Add(TTemplate template);
//        string CreateKey(TTemplate template);
//        string GetDefaultKey(TTemplate template);
//        string GetNextValidKeyIncrementForKey(string key);
//        void OnChildCountChanged(IInstantiation<TTemplate> child, int oldCount);
//        void RaiseAdded(IInstantiation<TTemplate> child, int oldCount);
//        void RaiseRemoved(IInstantiation<TTemplate> child, int oldCount);
//    }

    public interface IInstantiationCollection :
        IEnumerable<IInstantiation>,
             //, IParentable<InstantiationCollection>
             IParented
#if !AOT
                , INotifyCollectionChanged<IInstantiation>
#endif
    {

        event Action<IInstantiation, int> ChildCountChanged;
        new event NotifyCollectionChangedHandler<IInstantiation> CollectionChanged;

        IEnumerable<IInstantiation> Values { get; }
        int Count { get; }

        bool ContainsKey(string key);
        void Add(IInstantiation instantiation);
        void Add(IReadHandleBase<ITemplate> hTemplate);
        //void Add(ITemplate template);
        void Add<TTemplate>(TTemplate template) where TTemplate : ITemplate;
        void AddRange(params IInstantiation[] items);
        string CreateKey(ITemplate template);
        string GetDefaultKey(ITemplate template);
        string GetNextValidKeyIncrementForKey(string key);
        void OnChildCountChanged(IInstantiation child, int oldCount);
        void RaiseAdded(IInstantiation child, int oldCount);
        void RaiseRemoved(IInstantiation child, int oldCount);

        bool Remove(IInstantiation keyed);
        bool Remove(string key);
    }
}