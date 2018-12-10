using System;
using System.Collections.Generic;
using LionFire.DependencyInjection;
using LionFire.Referencing;

namespace LionFire.ObjectBus
{
    /// <summary>
    /// Gets handles for References via OBus
    /// </summary>
    public static class HandleProviderOBusExtensions
    {
        //public static H<T> ToHandle<T>(this string uriString) where T : class => InjectionContext.Current.GetService<IHandleProvider>().GetHandle<T>(new UriStringReference(uriString));

        //#region  TODO: see if reference implelements IProvidesHandleFromPath

        //public static H GetHandle(this IReference reference/*, object obj = null*/) => (H)reference.GetOBus().GetHandle<object>(reference/*, obj*/);
        //public static H<T> GetHandle<T>(this IReference reference/*, T obj = default(T)*/) => (reference is IProvidesHandleFromPath ph) ? ph.GetHandle<T>(reference.Path) : reference.GetOBus().GetHandle<T>(reference/*, obj*/);
        //public static H<T> ObjectToHandle<T>(this T obj) => throw new NotImplementedException("FUTURE: if obj != null, create a NamedObjectHandle and assign a random key");

        //public static R GetReadHandle(this IReference reference/*, object obj = null*/) => (R)reference.GetOBus().GetReadHandle<object>(reference);
        //public static R<T> GetReadHandle<T>(this IReference reference/*, T obj = default(T)*/) => reference.GetOBus().GetReadHandle<T>(reference);
        //public static R<T> ObjectToReadHandle<T>(this T obj) => throw new NotImplementedException("FUTURE: if obj != null, create a NamedObjectHandle and assign a random key");

        //#endregion

        public static C GetCollectionHandle(this IReference reference) => (C)reference.GetOBus().GetCollectionHandle<object>(reference);
        public static C<T> GetCollectionHandle<T>(this IReference reference) => (C<T>)reference.GetOBus().GetCollectionHandle<object>(reference);
        public static RC GetReadCollectionHandle(this IReference reference) => (RC)reference.GetOBus().GetReadCollectionHandle<object>(reference);
        public static RC<T> GetReadCollectionHandle<T>(this IReference reference) => (RC<T>)reference.GetOBus().GetReadCollectionHandle<object>(reference);


        public static H GetHandle(this IReference reference/*, object obj = null*/) => (H)reference.GetOBus().GetHandle<object>(reference/*, obj*/);
        public static H<T> GetHandle<T>(this IReference reference/*, T obj = default(T)*/) => reference.GetOBus().GetHandle<T>(reference/*, obj*/);
        public static H<T> ObjectToHandle<T>(this T obj) => throw new NotImplementedException("FUTURE: if obj != null, create a NamedObjectHandle and assign a random key");

        public static RH GetReadHandle(this IReference reference/*, object obj = null*/) => (RH)reference.GetOBus().GetReadHandle<object>(reference);
        public static RH<T> GetReadHandle<T>(this IReference reference/*, T obj = default(T)*/) => reference.GetOBus().GetReadHandle<T>(reference);
        public static RH<T> ObjectToReadHandle<T>(this T obj) => throw new NotImplementedException("FUTURE: if obj != null, create a NamedObjectHandle and assign a random key");

        //      public static H<T> ToHandle<T>(this IReferencable referencable, T obj = null)
        //          where T : class//, new()
        //      {
        //          return HandleProvider<T>.GetHandle(referencable.Reference, obj);
        //      }

        //      public static H ToHandle(this IReference reference, object obj , Type type)
        //{
        //	// TODO: Get HandleFactory<T> via reflection, perhaps only if not AOT
        //	return HandleProvider.GetHandle(reference, obj);
        //}

        //public static H CreateHandle(this IReference reference)
        //{
        //    return HandleFactory.CreateHandle(reference);
        //}
        //#if !AOT
        //        public static H <T> CreateHandle<T>(this IReference reference)
        //            where T : class, new()
        //        {
        //            return HandleFactory<T>.CreateHandle(reference);
        //        }
        //#endif
        //        public static H ToHandle(this IReferencable referencable)
        //        {
        //            return HandleProvider.GetHandle(referencable.Reference);
        //        }
    }

    //    /// <summary>
    // OLD - Migrated to a shim to HandleFactory<object>
    //    /// Caches Reference URIs to IHandles

    //    /// </summary>
    //    public static class HandleFactory
    //    {
    //        private static Dictionary<string, IH<object>> handlesByUri = new Dictionary<string, IH<object>>();
    //        private static ReaderWriterLockSlim handlesLock = new ReaderWriterLockSlim();
    //        public static bool ShareHandles = true;

    //        public static IH<object> GetHandle(IReference reference, object obj = null)
    //        {
    //            if (ShareHandles)
    //            {
    //                try
    //                {
    //                    handlesLock.EnterUpgradeableReadLock();
    //                    IH<object> handle = 
    //#if AOT
    //						(IHandle)
    //#endif
    //						handlesByUri.TryGetValue(reference.Uri);

    //                    if (handle != null)
    //                    {
    //                        return handle;
    //                    }
    //                    else
    //                    {
    //                        //return CreateHandle(reference); TODO
    //                        handle = new H<object>(reference, obj);
    //                        handlesLock.EnterWriteLock();
    //                        try
    //                        {
    //                            handlesByUri.Add(reference.Uri, handle);
    //                        }
    //                        finally
    //                        {
    //                            handlesLock.ExitWriteLock();
    //                        }
    //                        return handle;
    //                    }
    //                }
    //                finally
    //                {
    //                    if (handlesLock.IsUpgradeableReadLockHeld)
    //                    {
    //                        handlesLock.ExitUpgradeableReadLock();
    //                    }
    //                }
    //            }
    //            else
    //            {
    //                return CreateHandle(reference);
    //            }
    //        }

    //        public static IH<object> CreateHandle(IReference reference)
    //        {
    //            var handle = new H<object>(reference);

    //            //if (ShareHandles && handlesByUri.ContainsKey(reference.Uri)) // TODO: With locking
    //            //{
    //            //    handlesByUri.Add(reference.Uri, handle);
    //            //}
    //            return handle;
    //        }
    //    }


}
