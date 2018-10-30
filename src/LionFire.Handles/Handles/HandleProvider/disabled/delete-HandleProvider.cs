//using LionFire.ExtensionMethods;
//using LionFire.Referencing;
//using System;
//using System.Collections.Generic;
//using System.Threading;

//namespace LionFire.Referencing
//{
//    public static class HandleProvider
//    {
//        // UNTESTED: need to force this cast? (H)(object)
//        public static H GetHandle(this IReference reference, object obj = null) => (H)HandleProvider<object>.GetHandle(reference, obj);
//        public static H<T> GetHandle<T>(this IReference<T> reference, T obj = null)
//         where T : class
//             => HandleProvider<T>.GetHandle((IReference)reference, obj);
//    }

//    public static class HandleProvider<T>
//    where T : class//, new()
//    {
//        private static Dictionary<string, H<T>> handlesByUri = new Dictionary<string, H<T>>();
//        private static ReaderWriterLockSlim handlesLock = new ReaderWriterLockSlim();

//        public static H<T> GetHandle(IReference reference, T obj = null)
//        {
//#if DEBUG
//            if (obj != null) throw new NotImplementedException("obj!=null");
//#endif
//            if (HandlesConfig.ShareHandles)
//            {
//                try
//                {
//                    handlesLock.EnterUpgradeableReadLock();
//                    H<T> handle =
//#if AOT
//						(IHandle<T>)
//#endif
//                            handlesByUri.TryGetValue(reference.Key);

//                    if (handle != null)
//                    {
//                        return handle;
//                    }
//                    else
//                    {
//                        //return CreateHandle(reference); TODO
//                        handle = HandleFactory<T>.CreateHandle(reference, obj);

//                        handlesLock.EnterWriteLock();
//                        try
//                        {
//                            handlesByUri.Add(reference.Key, handle);
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
//                return HandleFactory<T>.CreateHandle(reference);
//            }
//        }
//    }
//}
