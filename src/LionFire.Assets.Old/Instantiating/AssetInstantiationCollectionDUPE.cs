#if false // DUPE
using LionFire.Collections;
using LionFire.Instantiating;
using LionFire.Ontology;
using LionFire.Referencing;
using Microsoft.Extensions.Logging;
using System;

namespace LionFire.Assets
{
    public class AssetInstantiationCollection2 : ListDictionary<string, IAssetInstantiation>
            //, IParented<InstantiationCollection>
            , IParented
#if !AOT
                , INotifyCollectionChanged<IAssetInstantiation>
#endif
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

        public string GetDefaultKey(ITemplateAsset template)
        {
            var pro = template as IDefaultInstanceKeyProvider;
            if (pro != null)
            {
                return pro.DefaultKey;
            }
            return template.GetType().Name.TemplateTypeNameToDisplayString(spaces: false);
        }

        public string CreateKey(ITemplateAsset template)
        {
            var key = GetDefaultKey(template);
            return GetNextValidKeyIncrementForKey(key);
        }

        public string GetNextValidKeyIncrementForKey(string key)
        {
            var i = 0;
            var result = key;
            while (base.ContainsKey(result))
            {
                i++;
                result = key + i;
            }
            return result;
        }

        #endregion

        protected override void OnAdded(IAssetInstantiation keyed)
        {
            base.OnAdded(keyed);
            var parented = keyed as IParented;
            if (parented != null) parented.Parent = this;

            CollectionChanged?.Invoke(NotifyCollectionChangedEventArgs<IAssetInstantiation>.Added(keyed));
        }

        protected override void OnRemoved(IAssetInstantiation keyed)
        {
            base.OnRemoved(keyed);
            var parented = keyed as IParented;
            if (parented != null) parented.Parent = null;

            CollectionChanged?.Invoke(NotifyCollectionChangedEventArgs<IAssetInstantiation>.Removed(keyed));
        }

        public void RaiseRemoved(IAssetInstantiation child, int oldCount)
        {
            CollectionChanged?.Invoke(NotifyCollectionChangedEventArgs<IAssetInstantiation>.Removed(child));
        }

        public void RaiseAdded(IAssetInstantiation child, int oldCount)
        {
            CollectionChanged?.Invoke(NotifyCollectionChangedEventArgs<IAssetInstantiation>.Added(child));
        }

        public event NotifyCollectionChangedHandler<IAssetInstantiation> CollectionChanged;

        public void Add(ITemplateAsset template)
        {
            throw new NotImplementedException("TOPORT");
//            IAssetInstantiation instantiation = template.CreateAssetInstantiation();
//            instantiation.Key = CreateKey(template);
//#if SanityChecks // TEMP
//            if (instantiation.TemplateAsset == null) throw new UnreachableCodeException("instantiation.Template==null");
//#endif
//            this.Add(instantiation);
        }
#if !AOT
        public void Add(RH<ITemplateAsset> hTemplate) => Add(new Instantiation(hTemplate));
#endif

        //public void Add(string templateAssetPath)
        //{
        //    base.Add(new Instantiation(templateAssetPath));
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
            Add((IAssetInstantiation)instantiation);
        }

        public void OnChildCountChanged(IAssetInstantiation child, int oldCount)
        {
            Action<IAssetInstantiation, int> ev = ChildCountChanged;
            if (ev != null) ev(child, oldCount);
        }

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
#endif