using System;
using System.Collections.Specialized;
using System.Text;
//using JsonExSerializer.TypeConversion;
using LionFire.Collections;
using LionFire.Structures;
//using LionFire.ObjectBus;
using LionFire.Ontology;
using Microsoft.Extensions.Logging;
using LionFire.Referencing;
using LionFire.Persistence;

namespace LionFire.Instantiating
{

    public class InstantiationCollection : ListDictionary<string, IInstantiation>
 , IInstantiationCollection
 //, INotifyingDictionary<string, IInstantiation>
 //, INestedNotifyingCollection
 //: Dictionary<string, IInstantiation>
 //, IEnumerable<IInstantiation>
 //where ITemplate : ITemplate
    {

        #region Parent

        [Ignore]
        public object Parent { get; set; }
        //object IParented.Parent { get{return this.Parent;} set{this.Parent = (InstantiationCollection)value;} }
        //public InstantiationCollection Parent { get; set; }

        #endregion

        //public IEnumerator<IInstantiation> GetEnumerator()  base
        //{
        //    var x = (IEnumerable<KeyValuePair<string, IInstantiation>>)this;
        //    foreach (var item in x)
        //    {
        //        yield return item.Value;
        //    }
        //    foreach (var listItem in list)
        //    {
        //        yield return listItem;
        //    }
        //}

        #region Keys

        public string GetDefaultKey(ITemplate template)
        {
            var pro = template as IDefaultInstanceKeyProvider;
            if (pro != null)
            {
                return pro.DefaultKey;
            }
            return template.GetType().Name.TemplateTypeNameToDisplayString(spaces: false);
        }

        public string CreateKey(ITemplate template)
        {
            string key = GetDefaultKey(template);
            return GetNextValidKeyIncrementForKey(key);
        }

        public string GetNextValidKeyIncrementForKey(string key)
        {
            int i = 0;
            string result = key;
            while (base.ContainsKey(result))
            {
                i++;
                result = key + i;
            }
            return result;
        }

        #endregion

        #region Events

        protected override void OnAdded(IInstantiation keyed)
        {
            base.OnAdded(keyed);
            if (keyed is IParented parented) parented.Parent = this;
            CollectionChanged?.Invoke(NotifyCollectionChangedEventArgs<IInstantiation>.Added(keyed));
        }

        protected override void OnRemoved(IInstantiation keyed)
        {
            base.OnRemoved(keyed);
            if (keyed is IParented parented) parented.Parent = null;
            CollectionChanged?.Invoke(NotifyCollectionChangedEventArgs<IInstantiation>.Removed(keyed));
        }

        public void RaiseRemoved(IInstantiation child, int oldCount) => CollectionChanged?.Invoke(NotifyCollectionChangedEventArgs<IInstantiation>.Removed(child));

        public void RaiseAdded(IInstantiation child, int oldCount) => CollectionChanged?.Invoke(NotifyCollectionChangedEventArgs<IInstantiation>.Added(child));

        public event NotifyCollectionChangedHandler<IInstantiation> CollectionChanged;

        public void OnChildCountChanged(IInstantiation child, int oldCount)
        {
            var ev = ChildCountChanged;
            if (ev != null) ev(child, oldCount);
        }

#if AOT
        public event Action_IInstantiation_int ChildCountChanged;
#else
        public event Action<IInstantiation, int> ChildCountChanged;
#endif

        #endregion

        #region Add

        public void Add(ITemplate template)
        {
            var instantiation = template.CreateInstantiation();
            instantiation.Key = CreateKey(template);
#if SanityChecks // TEMP
            if (instantiation.Template == null) throw new UnreachableCodeException("instantiation.Template==null");
#endif
            this.Add(instantiation);
        }
#if !AOT
        public void Add(IReadHandleBase<ITemplate> hTemplate) => Add(new Instantiation<ITemplate>(hTemplate));
#endif

        //public void Add(string templatePath)
        //{
        //    base.Add(new Instantiation(templatePath));
        //}

//        public void Add(IInstantiation instantiation)
//        {
//#if SanityChecks
//            if (instantiation.Parameters != null && instantiation.Parameters != instantiation)
//            {
//                l.Warn("InstantiationCollection.Add: instantiation.Parameters != null && instantiation.Parameters != instantiation.  instantiation: " +
//                    instantiation.ToString() + " of type " + instantiation.GetType().Name + ", Parameters: " + (instantiation.Parameters == null ? "null" : instantiation.Parameters.GetType().Name));
//            }
//#endif
//            base.Add((IInstantiation)instantiation);
//        }

        #endregion

        #region Misc

        private static readonly ILogger l = Log.Get();

        #endregion

    }


#if false
    public class InstantiationCollection<TTemplate> : ListDictionary<string, IInstantiation<TTemplate>>
    , IInstantiationCollection<TTemplate>
    //, INotifyingDictionary<string, IInstantiation>
    //, INestedNotifyingCollection
    //: Dictionary<string, IInstantiation>
    //, IEnumerable<IInstantiation>
    where TTemplate : ITemplate
    {

    #region Parent

        [Ignore]
        public object Parent { get; set; }
        //object IParented.Parent { get{return this.Parent;} set{this.Parent = (InstantiationCollection)value;} }
        //public InstantiationCollection Parent { get; set; }

    #endregion

        //public IEnumerator<IInstantiation<TTemplate>> GetEnumerator()  base
        //{
        //    var x = (IEnumerable<KeyValuePair<string, IInstantiation<TTemplate>>>)this;
        //    foreach (var item in x)
        //    {
        //        yield return item.Value;
        //    }
        //    foreach (var listItem in list)
        //    {
        //        yield return listItem;
        //    }
        //}

    #region Keys

        public string GetDefaultKey(TTemplate template)
        {
            var pro = template as IDefaultInstanceKeyProvider;
            if (pro != null)
            {
                return pro.DefaultKey;
            }
            return template.GetType().Name.TemplateTypeNameToDisplayString(spaces: false);
        }

        public string CreateKey(TTemplate template)
        {
            string key = GetDefaultKey(template);
            return GetNextValidKeyIncrementForKey(key);
        }

        public string GetNextValidKeyIncrementForKey(string key)
        {
            int i = 0;
            string result = key;
            while (base.ContainsKey(result))
            {
                i++;
                result = key + i;
            }
            return result;
        }

    #endregion

        protected override void OnAdded(IInstantiation<TTemplate> keyed)
        {
            base.OnAdded(keyed);
            if (keyed is IParented parented) parented.Parent = this;
            CollectionChanged?.Invoke(NotifyCollectionChangedEventArgs<IInstantiation<TTemplate>>.Added(keyed));
        }

        protected override void OnRemoved(IInstantiation<TTemplate> keyed)
        {
            base.OnRemoved(keyed);
            if (keyed is IParented parented) parented.Parent = null;
            CollectionChanged?.Invoke(NotifyCollectionChangedEventArgs<IInstantiation<TTemplate>>.Removed(keyed));
        }

        public void RaiseRemoved(IInstantiation<TTemplate> child, int oldCount) => CollectionChanged?.Invoke(NotifyCollectionChangedEventArgs<IInstantiation<TTemplate>>.Removed(child));

        public void RaiseAdded(IInstantiation<TTemplate> child, int oldCount) => CollectionChanged?.Invoke(NotifyCollectionChangedEventArgs<IInstantiation<TTemplate>>.Added(child));

        public event NotifyCollectionChangedHandler<IInstantiation<TTemplate>> CollectionChanged;

        public void Add(TTemplate template)
        {
            var instantiation = template.CreateInstantiation();
            instantiation.Key = CreateKey(template);
#if SanityChecks // TEMP
            if (instantiation.Template == null) throw new UnreachableCodeException("instantiation.Template==null");
#endif
            this.Add(instantiation);
        }
#if !AOT
        public void Add(IReadHandleBase<TTemplate> hTemplate) => Add(new Instantiation<TTemplate>(hTemplate));
#endif

        //public void Add(string templatePath)
        //{
        //    base.Add(new Instantiation(templatePath));
        //}


        public void Add(IInstantiation instantiation)
        {
#if SanityChecks
            if (instantiation.Parameters != null && instantiation.Parameters != instantiation)
            {
                l.Warn("InstantiationCollection.Add: instantiation.Parameters != null && instantiation.Parameters != instantiation.  instantiation: " +
                    instantiation.ToString() + " of type " + instantiation.GetType().Name + ", Parameters: " + (instantiation.Parameters == null ? "null" : instantiation.Parameters.GetType().Name));
            }
#endif
            Add((IInstantiation<TTemplate>)instantiation);
        }

        public void OnChildCountChanged(IInstantiation<TTemplate> child, int oldCount)
        {
            var ev = ChildCountChanged;
            if (ev != null) ev(child, oldCount);
        }

#if AOT
        public event Action_IInstantiation<TTemplate>_int ChildCountChanged;
#else
        public event Action<IInstantiation<TTemplate>, int> ChildCountChanged;
#endif

    #region Misc

        private static readonly ILogger l = Log.Get();

    #endregion

    }
#endif
}
