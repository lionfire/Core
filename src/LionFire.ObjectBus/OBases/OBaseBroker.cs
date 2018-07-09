using LionFire.Referencing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire.ObjectBus
{
    public static class OBaseBroker
    {
        #region OBases

        public static IEnumerable<IOBase> GetOBases(this IReference reference)
        {
            //var sets = new List<IOBase>();

            foreach (IOBaseProvider osp in reference.GetOBaseProviders())
            {
                foreach (var obase in osp.GetOBases(reference))
                {
                    yield return obase;
                }
                //sets.AddRange(osp.GetOBases(reference));
            }

            //return sets;
        }

        //public static IOBase GetOBase(this IReference reference)
        //{
        //    return GetOBases(reference).SingleOrDefault();
        //}

        #endregion


        //    //public IEnumerable<IObjectStore> GetObjectStores(VosObject obj)
        //    //{
        //    //    var objectStores = obj.AsType<IList<IObjectStore>>();
        //    //    if (objectStores != null)
        //    //    {
        //    //        return objectStores;
        //    //    }
        //    //}

        //    public IEnumerable<IObjectStore> GetObjectStores(IReference reference)
        //    {
        //        List<IObjectStore> objectStores = new List<IObjectStore>();

        //        objectStores.AddRange(SchemeBroker.Instance.ObjectStoreProviders.Select(osp.GetObjectStores(reference)));

        //        List<IObjectStore> objectStores = new List<IObjectStore>();

        //        foreach (var osp in SchemeBroker.Instance.ObjectStoreProviders)
        //        {

        //        }
        //    }


        //public static class ObjectStoreBrokerExtensions
        //{
        //    public static IEnumerable<IObjectStore> GetObjectStores(this IReference reference)
        //    {
        //        return 
        //    }
        //}

    }

}
