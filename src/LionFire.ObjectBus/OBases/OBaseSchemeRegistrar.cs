#if UNUSED // maybe later.  See Review comment in OBaseResolverService
// TODO: Revive this, maybe with a base class that resolves 
using System.Collections.Generic;
using LionFire.Referencing;
using LionFire.Structures;

namespace LionFire.Structures
{
    public abstract class ManyToManyRegistrar<TItem, TKey> // MOVE to LionFire.Core
    {
        public abstract IEnumerable<TKey> KeysForItem(TItem item);

        protected IEnumerable<TItem> items;

        public Dictionary<TKey, List<TItem>> Dictionary { get; set; }

        public void InitRegistrar()
        {
            Dictionary = new Dictionary<TKey, List<TItem>>();

            foreach (var item in items)
            {
                foreach (var key in KeysForItem(item))
                {
                    if (!Dictionary.ContainsKey(key))
                    {
                        Dictionary.Add(key, new List<TItem>());
                    }
                    Dictionary[key].Add(item);
                }
            }
        }
    }

    public abstract class ManyToOneRegistrar<TItem, TKey> // MOVE to LionFire.Core
    {
        public abstract IEnumerable<TKey> KeysForItem(TItem item);

        protected IEnumerable<TItem> items;

        public IReadOnlyDictionary<TKey, TItem> Dictionary { get; set; }

        public void InitRegistrar()
        {
            var dict = new Dictionary<TKey, TItem>();

            foreach (var item in items)
            {
                foreach (var key in KeysForItem(item))
                {
                    if (dict.ContainsKey(key))
                    {
                        throw new AlreadyException($"{key} is already registered by {keys[key].GetType().Name}.  Only one is allowed to register this.");
                    }

                    dict.Add(key, item);
                }
            }
            Dictionary = dict;
        }
    }
}

namespace LionFire.ObjectBus
{
    public class OBaseSchemeRegistrar : ManyToOneRegistrar<IOBaseProvider, string>, IOBaseResolverService
    {
        public override IEnumerable<string> KeysForItem(IOBaseProvider item) => item.SupportedUriSchemes;

        public OBaseSchemeRegistrar(IEnumerable<IOBaseProvider> obaseProviders)
        {
            items = obaseProviders;
            InitRegistrar();
        }

        public (IOBaseProvider obaseProvider, IOBase obase) Resolve(IReference input) => throw new System.NotImplementedException();
    }


    //    // THREADUNSAFE - perhaps this is ok as long as there are no writes after initialization: TODO: freeze after config, and during reconfig, lock it except for current thread
    //    public class OBaseSchemeBroker
    //    {
    //        #region (Static) Singleton Accessor

    //        public static OBaseSchemeBroker Instance => Singleton<OBaseSchemeBroker>.Instance;  // TODO - DI

    //        #endregion

    //        #region Fields

    //        internal IEnumerable<IOBaseProvider> ObjectStoreProviders => objectStores.SelectMany(kvp => kvp.Value);

    //        // FUTURE: Allow precedence options for this resolution
    //        private MultiValueDictionary<string, IOBaseProvider> objectStores = new MultiValueDictionary<string, IOBaseProvider>();

    //        #endregion

    //        public void RegisterAvailableProviders()
    //        {
    //            foreach (var obaseProvider in InjectionContext.Current.GetService<IEnumerable<IOBaseProvider>>())
    //            {
    //                foreach (var scheme in obaseProvider.SupportedUriSchemes)
    //                {
    //                    objectStores.Add(scheme, obaseProvider);
    //                }
    //            }
    //        }

    //        //// OLD - use IAppHost.AddSingleton<IOBaseProvider, ImplType>() and IAppHost.AddObjectBus() instead,
    //        //public void Register<T>(T obaseProvider)
    //        //    where T : IOBaseProvider
    //        //{
    //        //    throw new NotImplementedException("OLD");

    //        //    foreach (var scheme in obaseProvider.UriSchemes)
    //        //    {
    //        //        objectStores.Add(scheme, obaseProvider);
    //        //    }
    //        //}

    //        public IEnumerable<IOBaseProvider> this[string scheme] => objectStores.TryGetValue(scheme, returnEmptySet: true);

    //    }
}

#endif