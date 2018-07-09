using LionFire.Referencing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire.ObjectBus
{
    public static class OBaseProviderBroker
    {
        #region OBaseProviders

        public static IEnumerable<IOBaseProvider> GetOBaseProviders(this IReference reference)
        {
            //reference.DefaultObjectStoreProvider?
            var osps = SchemeBroker.Instance[reference.Scheme];
            return osps;
        }

        public static IOBaseProvider GetOBaseProvider(this IReference reference)
        {
            var osps = SchemeBroker.Instance[reference.Scheme];
            return osps.SingleOrDefault();
        }

        #endregion
    }
}
