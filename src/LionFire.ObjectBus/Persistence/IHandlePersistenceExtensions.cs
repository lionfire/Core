using LionFire.Referencing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire.ObjectBus
{
    

    //public interface IHandleInternal<ObjectType> : IHandle<ObjectType>
    //    where ObjectType : class, new()
    //{

    //}

    public static class OBusSettings
    {
        public static bool AllowOverwriteOnSave = true;
    }

    // I am liking the state tracking within HandleBase2 as an ActiveRecord kind of object.
    public static class IHandlePersistenceExtensions
    {
        public static bool TryDelete(this IHandle handle)
    //where ObjectType : class, new()
        {
            bool result = OBus.TryDelete(handle.Reference);
            if (result)
            {
                ((IHandlePersistenceEvents)handle).OnDeleted();
            }
            return result;
        }

#if !AOT
        public static void Save<ObjectType>(this H<ObjectType> handle) // UNUSED?  Alternative to HandleBase?
            where ObjectType : class, new()
        {
            throw new NotImplementedException("TOPORT");

            //var obj = handle.ObjectField;

            //if (obj == default(ObjectType))
            //{
            //    handle.TryDelete();
            //    return;
            //}

            //if (OBusSettings.AllowOverwriteOnSave)
            //{
            //    OBus.Set(handle.Reference, obj);
            //}
            //else
            //{
            //    if (handle.IsPersisted)
            //    {
            //        OBus.Set(handle.Reference, obj); // TODO: Use update instead to make sure it wasn't deleted out from under us

            //        // FUTURE: Some kind of threadsafe versioning logic to make sure the object wasn't updated from under us.
            //    }
            //    else
            //    {
            //        OBus.Create(handle.Reference, obj); // Throws if already exists
            //    }
            //}

            //((IHandlePersistenceEvents)handle).OnSaved(); // Sets IsPersisted = true
        }
#endif
    }
}
