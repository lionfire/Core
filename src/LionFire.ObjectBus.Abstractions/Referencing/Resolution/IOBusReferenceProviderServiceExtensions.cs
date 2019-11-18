//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using LionFire.Dependencies;
//using LionFire.Referencing;

//namespace LionFire.ObjectBus
//{
//    public static class IOBusReferenceProviderServiceExtensions
//    {
//        // FUTURE: More sophisticated?  More deterministic?  For now it just returns the first reference it can.
//        public static IReference ToReference(this string uriString, bool strictMode = true) =>
//         DependencyContext.Current.GetService<IOBusReferenceProviderService>().ResolveAll(uriString, strictMode).Select(rp => rp.TryGetReference(uriString)).Where(r => r != null).FirstOrDefault();

//    }
//}
