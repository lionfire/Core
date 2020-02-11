using System;
using Microsoft.Extensions.Logging;
using System.Collections.Specialized;
using System.Text;
//using JsonExSerializer.TypeConversion;
using LionFire.Collections;
using LionFire.Structures;
using LionFire.Ontology;
using LionFire.Instantiating;
using LionFire.Referencing;

namespace LionFire.Assets
{
    public delegate void Action_IInstantiation_int(IAssetInstantiation instantiation, int num);

    
    public class InstantiationCollectionBase<TValue> : ListDictionary<string, TValue>
            //, IParented<InstantiationCollection>
            , IParented
        where TValue : IKeyed<string>
    {
        #region Parent

        [Ignore]
        public object Parent { get; set; }
        //object IParented.Parent { get{return this.Parent;} set{this.Parent = (InstantiationCollection)value;} }
        //public InstantiationCollection Parent { get; set; }

        #endregion

        #region Keys

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

        
        public string GetDefaultKey(ITemplate template)
        {
            if (template is IDefaultInstanceKeyProvider pro)
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

        #endregion
    }

    public class AssetInstantiationCollection : InstantiationCollectionBase<IAssetInstantiation>
        
#if !AOT
                , INotifyCollectionChanged<IAssetInstantiation>
#endif
    //, INotifyingDictionary<string, IInstantiation>
    //, INestedNotifyingCollection
    //: Dictionary<string, IInstantiation>
    //, IEnumerable<IInstantiation>
    {

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

        protected override void OnAdded(IAssetInstantiation keyed)
        {
            base.OnAdded(keyed);
            if (keyed is IParented parented) parented.Parent = this;

            CollectionChanged?.Invoke(NotifyCollectionChangedEventArgs<IAssetInstantiation>.Added(keyed));
        }

        protected override void OnRemoved(IAssetInstantiation keyed)
        {
            base.OnRemoved(keyed);
            if (keyed is IParented parented) parented.Parent = null;

            CollectionChanged?.Invoke(NotifyCollectionChangedEventArgs<IAssetInstantiation>.Removed(keyed));
        }

        public void RaiseRemoved(IAssetInstantiation child, int oldCount) => CollectionChanged?.Invoke(NotifyCollectionChangedEventArgs<IAssetInstantiation>.Removed(child));

        public void RaiseAdded(IAssetInstantiation child, int oldCount) => CollectionChanged?.Invoke(NotifyCollectionChangedEventArgs<IAssetInstantiation>.Added(child));

        public event NotifyCollectionChangedHandler<IAssetInstantiation> CollectionChanged;

        //public void Add(string templateAssetPath)
        //{
        //    base.Add(new Instantiation(templateAssetPath));
        //}

#if TOPORT
            // Not sure how to do these yet:

            public void Add(ITemplateAsset template)
        {
            var instantiation = template.CreateAssetInstantiation();
            instantiation.Key = CreateKey(template);
#if SanityChecks // TEMP
            if (instantiation.TemplateAsset == null) throw new UnreachableCodeException("instantiation.Template==null");
#endif
            this.Add(instantiation);
        }
#if !AOT
        public void Add(RH<ITemplateAsset> hTemplate) => Add(new Instantiation(hTemplate));
#endif

        //        public void Add(IAssetInstantiation instantiation)
        //        {
        //#if SanityChecks
        //            if (instantiation.Parameters != null && instantiation.Parameters != instantiation)
        //            {
        //                l.Warn("InstantiationCollection.Add: instantiation.Parameters != null && instantiation.Parameters != instantiation.  instantiation: " +
        //                    instantiation.ToString() + " of type " + instantiation.GetType().Name + ", Parameters: " + (instantiation.Parameters == null ? "null" : instantiation.Parameters.GetType().Name));
        //            }
        //#endif
        //            Add((IAssetInstantiation)instantiation);
        //        }
#endif
        //#endif

        public void OnChildCountChanged(IAssetInstantiation child, int oldCount) => ChildCountChanged?.Invoke(child, oldCount);

#if AOT
        public event Action_IInstantiation_int ChildCountChanged;
#else
        public event Action<IAssetInstantiation, int> ChildCountChanged;
#endif

#region Misc

        private static ILogger l = Log.Get();

#endregion

    }
}