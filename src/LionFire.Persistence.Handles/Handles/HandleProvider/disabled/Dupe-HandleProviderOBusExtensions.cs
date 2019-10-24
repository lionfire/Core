//using LionFire.DependencyInjection;
//using LionFire.ObjectBus;
//using LionFire.Referencing;
//using System;

//namespace LionFire.Referencing
//{
//    //    /// <summary>
//    // OLD - Migrated to a shim to HandleFactory<object>
//    //    /// Caches Reference URIs to IHandles

//    //    /// </summary>
//    //    public static class HandleFactory
//    //    {
//    //        private static Dictionary<string, IH<object>> handlesByUri = new Dictionary<string, IH<object>>();
//    //        private static ReaderWriterLockSlim handlesLock = new ReaderWriterLockSlim();
//    //        public static bool ShareHandles = true;

//    //        public static IH<object> ToHandle(IReference reference, object obj = null)
//    //        {
//    //            if (ShareHandles)
//    //            {
//    //                try
//    //                {
//    //                    handlesLock.EnterUpgradeableReadLock();
//    //                    IH<object> handle = 
//    //#if AOT
//    //						(IHandle)
//    //#endif
//    //						handlesByUri.TryGetValue(reference.Uri);

//    //                    if (handle != null)
//    //                    {
//    //                        return handle;
//    //                    }
//    //                    else
//    //                    {
//    //                        //return CreateHandle(reference); TODO
//    //                        handle = new H<object>(reference, obj);
//    //                        handlesLock.EnterWriteLock();
//    //                        try
//    //                        {
//    //                            handlesByUri.Add(reference.Uri, handle);
//    //                        }
//    //                        finally
//    //                        {
//    //                            handlesLock.ExitWriteLock();
//    //                        }
//    //                        return handle;
//    //                    }
//    //                }
//    //                finally
//    //                {
//    //                    if (handlesLock.IsUpgradeableReadLockHeld)
//    //                    {
//    //                        handlesLock.ExitUpgradeableReadLock();
//    //                    }
//    //                }
//    //            }
//    //            else
//    //            {
//    //                return CreateHandle(reference);
//    //            }
//    //        }

//    //        public static IH<object> CreateHandle(IReference reference)
//    //        {
//    //            var handle = new H<object>(reference);

//    //            //if (ShareHandles && handlesByUri.ContainsKey(reference.Uri)) // TODO: With locking
//    //            //{
//    //            //    handlesByUri.Add(reference.Uri, handle);
//    //            //}
//    //            return handle;
//    //        }
//    //    }

//    /// <summary>
//    /// REVIEW - keep this?  What about ToReadHandle?
//    /// </summary>
//    public static class HandleProviderOBusExtensions
//    {
//        //public static H<TValue> ToHandle<TValue>(this string uriString) where TValue : class => DependencyContext.Current.GetService<IHandleProvider>().ToHandle<TValue>(new UriStringReference(uriString));

//        public static H<object> ToHandle(this IReference reference, object obj = null) => reference.GetOBase().ToHandle<object>(reference);
//        public static H<TValue> ToHandle<TValue>(this IReference reference, object obj = null) => reference.GetOBase().ToHandle<object>(reference);

//        //      public static H<TValue> ToHandle<TValue>(this IReferencable referencable, TValue obj = null)
//        //          where TValue : class//, new()
//        //      {
//        //          return HandleProvider<TValue>.ToHandle(referencable.Reference, obj);
//        //      }

//        //      public static H ToHandle(this IReference reference, object obj , Type type)
//        //{
//        //	// TODO: Get HandleFactory<TValue> via reflection, perhaps only if not AOT
//        //	return HandleProvider.ToHandle(reference, obj);
//        //}

//        //public static H CreateHandle(this IReference reference)
//        //{
//        //    return HandleFactory.CreateHandle(reference);
//        //}
//        //#if !AOT
//        //        public static H <TValue> CreateHandle<TValue>(this IReference reference)
//        //            where TValue : class, new()
//        //        {
//        //            return HandleFactory<TValue>.CreateHandle(reference);
//        //        }
//        //#endif
//        //        public static H ToHandle(this IReferencable referencable)
//        //        {
//        //            return HandleProvider.ToHandle(referencable.Reference);
//        //        }
//    }
//}
