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
//        public static H ToHandle(this IReference reference, object obj = null) => (H)HandleProvider<object>.ToHandle(reference, obj);
//        public static H<TValue> ToHandle<TValue>(this IReference<TValue> reference, TValue obj = null)
//         where TValue : class
//             => HandleProvider<TValue>.ToHandle((IReference)reference, obj);
//    }

//    public static class HandleProvider<TValue>
//    where TValue : class//, new()
//    {
//        private static Dictionary<string, H<TValue>> handlesByUri = new Dictionary<string, H<TValue>>();
//        private static ReaderWriterLockSlim handlesLock = new ReaderWriterLockSlim();

//        public static H<TValue> ToHandle(IReference reference, TValue obj = null)
//        {
//#if DEBUG
//            if (obj != null) throw new NotImplementedException("obj!=null");
//#endif
//            if (HandlesConfig.ShareHandles)
//            {
//                try
//                {
//                    handlesLock.EnterUpgradeableReadLock();
//                    H<TValue> handle =
//#if AOT
//						(IHandle<TValue>)
//#endif
//                            handlesByUri.TryGetValue(reference.Key);

//                    if (handle != null)
//                    {
//                        return handle;
//                    }
//                    else
//                    {
//                        //return CreateHandle(reference); TODO
//                        handle = HandleFactory<TValue>.CreateHandle(reference, obj);

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
//                return HandleFactory<TValue>.CreateHandle(reference);
//            }
//        }
//    }
//}

