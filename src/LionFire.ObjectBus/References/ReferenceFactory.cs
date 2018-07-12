using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire.Referencing
{
    
    public static class ReferenceFactory
    {
        //private static Dictionary<string, IReferenceFactory> referenceFactoriesByUriScheme = new Dictionary<string, IReferenceFactory>();

        //private static void RegisterType<T>() where T : IReferenceFactory, new()
        //{
        //    var objectStore = new T();
        //    foreach (var scheme in objectStore.UriSchemes)
        //    {
        //        referenceFactoriesByUriScheme.Add(scheme, objectStore);
        //    }
        //}


        //static ReferenceFactory()
        //{
        //    RegisterType<FsOBase>();
        //    RegisterType<Vos>();
        //}

        public static IReference ToReference(this string uri)
        {
            int colonIndex = uri.IndexOf(':');
            if (colonIndex < 0)
            {
                throw new ArgumentException("Scheme missing");
            }

            string scheme = uri.Substring(0, colonIndex);

            // TOREFACTOR TODO: Use LionFire.Core DI to get SchemeBroker here.
            var osp = OBaseSchemeBroker.Instance[scheme].FirstOrDefault(); // TODO: Review First

            //if (!referenceFactoriesByUriScheme.ContainsKey(scheme))
            if (osp == null)
            {
                throw new ArgumentException("No ReferenceFactory registered for scheme: " + scheme);
            }

            //var referenceFactory = referenceFactoriesByUriScheme[scheme];
            //return referenceFactory.ToReference(uri);

            return osp.ToReference(uri);
        }
    }
}
