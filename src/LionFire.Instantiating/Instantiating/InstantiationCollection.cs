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
            IDefaultInstanceKeyProvider pro = template as IDefaultInstanceKeyProvider;
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

        protected override void OnAdded(IInstantiation keyed)
        {
            base.OnAdded(keyed);
            var parented = keyed as IParented;
            if (parented != null) parented.Parent = this;

            CollectionChanged?.Invoke(NotifyCollectionChangedEventArgs<IInstantiation>.Added(keyed));
        }

        protected override void OnRemoved(IInstantiation keyed)
        {
            base.OnRemoved(keyed);
            var parented = keyed as IParented;
            if (parented != null) parented.Parent = null;

            CollectionChanged?.Invoke(NotifyCollectionChangedEventArgs<IInstantiation>.Removed(keyed));
        }

        public void RaiseRemoved(IInstantiation child, int oldCount)
        {
            CollectionChanged?.Invoke(NotifyCollectionChangedEventArgs<IInstantiation>.Removed(child));
        }

        public void RaiseAdded(IInstantiation child, int oldCount)
        {
            CollectionChanged?.Invoke(NotifyCollectionChangedEventArgs<IInstantiation>.Added(child));
        }

        public event NotifyCollectionChangedHandler<IInstantiation> CollectionChanged;

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
        public void Add(RH<ITemplate> hTemplate) => Add(new Instantiation(hTemplate));
#endif

        //public void Add(string templatePath)
        //{
        //    base.Add(new Instantiation(templatePath));
        //}


        public void Add(Instantiation instantiation)
        {
#if SanityChecks
            if (instantiation.Parameters != null && instantiation.Parameters != instantiation)
            {
                l.Warn("InstantiationCollection.Add: instantiation.Parameters != null && instantiation.Parameters != instantiation.  instantiation: " +
                    instantiation.ToString() + " of type " + instantiation.GetType().Name + ", Parameters: " + (instantiation.Parameters == null ? "null" : instantiation.Parameters.GetType().Name));
            }
#endif
            Add((IInstantiation)instantiation);
        }

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

        #region Misc

        private static ILogger l = Log.Get();

        #endregion

    }

}
