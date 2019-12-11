//using LionFire.Persistence.Handles;
//using LionFire.Referencing;
//using System;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.Text;

//namespace LionFire.Providers
//{
//    public class PersistenceProvidersRegistrar<TReference>
//        where TReference : IReference
//    {
//        ConcurrentDictionary<string, IReadHandleProvider> readHandleProviders = new ConcurrentDictionary<string, IReadHandleProvider>();

//        public void Register(object handleProvider, string name = "")
//        {
//            if(handleProvider is IReadHandleProvider<TReference> rhp) readHandleProviders.AddOrUpdate(typeof(TReference), rhp);

//        }
//    }
//}
