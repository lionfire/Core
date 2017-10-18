using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LionFire.Collections;

namespace LionFire.ObjectBus
{
    // TODO: Revamp this to a IReference to IReadHandle/IWritableHandle registrar, with no involvement of OBases.

    // THREADUNSAFE - perhaps this is ok as long as there are no writes after initialization: TODO: freeze after config, and during reconfig, lock it except for current thread
    public class SchemeBroker
    {
        #region (Static) Singleton Accessor

        public static SchemeBroker Instance { get { return Singleton<SchemeBroker>.Instance; } }

        #endregion

        #region Construction

        static SchemeBroker()
        {
            OBusConfig.Initialize();
        }

        #endregion

        #region Fields

        internal IEnumerable<IOBaseProvider> ObjectStoreProviders
        {
            get { return objectStores.SelectMany(kvp => kvp.Value); }
        }

        // FUTURE: Allow precedence options for this resolution
        private MultiValueDictionary<string, IOBaseProvider> objectStores = new MultiValueDictionary<string, IOBaseProvider>();

        #endregion

        public void Register<T>(T objectStoreProvider)
            where T : IOBaseProvider
        {
            foreach (var scheme in objectStoreProvider.UriSchemes)
            {
                objectStores.Add(scheme, objectStoreProvider);
            }
        }

        public IEnumerable<IOBaseProvider> this[string scheme]
        {
            get
            {
                return objectStores.TryGetValue(scheme, returnEmptySet:true);
            }
        }

    }
}
