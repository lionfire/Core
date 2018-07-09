using LionFire.Referencing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace LionFire.ObjectBus
{

#if !AOT
    /// <summary>
    /// Caches Reference URIs to IHandles
    /// </summary>
    /// <typeparam name="T">Type of Object</typeparam>
    public static class HandleFactory<T>
        where T : class//, new()
    {
        private static Dictionary<string, IHandle<T>> handlesByUri = new Dictionary<string, IHandle<T>>();
        private static ReaderWriterLockSlim handlesLock = new ReaderWriterLockSlim();
        public static bool ShareHandles = true;

        public static IHandle<T> CreateHandle(IReference reference, T obj = null)
            //where T:class
        {
            IHandle<T> handle = new Handle<T>(reference, obj);
            return handle;
        }

        public static IHandle<T> GetHandle(IReference reference, T obj = null)
            //where T:class
        {
#if DEBUG
            if (obj != null) throw new NotImplementedException("obj!=null");
#endif
            if (ShareHandles)
            {
                try
                {
                    handlesLock.EnterUpgradeableReadLock();
                    IHandle<T> handle = 
#if AOT
						(IHandle<T>)
#endif
							handlesByUri.TryGetValue(reference.Uri);

                    if (handle != null)
                    {
                        return handle;
                    }
                    else
                    {
                        //return CreateHandle(reference); TODO
                        handle = CreateHandle(reference, obj);
                        
                        handlesLock.EnterWriteLock();
                        try
                        {
                            handlesByUri.Add(reference.Uri, handle);
                        }
                        finally
                        {
                            handlesLock.ExitWriteLock();
                        }
                        return handle;
                    }
                }
                finally
                {
                    if (handlesLock.IsUpgradeableReadLockHeld)
                    {
                        handlesLock.ExitUpgradeableReadLock();
                    }
                }
            }
            else
            {
                return CreateHandle(reference);
            }
        }

        public static IHandle<T> CreateHandle(IReference reference)
        {
            var handle = new Handle<T>(reference);
            return handle;
        }
    }
#endif

    /// <summary>
    /// Caches Reference URIs to IHandles
    /// </summary>
    public static class HandleFactory
    {
        private static Dictionary<string, IHandle> handlesByUri = new Dictionary<string, IHandle>();
        private static ReaderWriterLockSlim handlesLock = new ReaderWriterLockSlim();
        public static bool ShareHandles = true;

        public static IHandle GetHandle(IReference reference, object obj = null)
        {
            if (ShareHandles)
            {
                try
                {
                    handlesLock.EnterUpgradeableReadLock();
                    IHandle handle = 
#if AOT
						(IHandle)
#endif
						handlesByUri.TryGetValue(reference.Uri);

                    if (handle != null)
                    {
                        return handle;
                    }
                    else
                    {
                        //return CreateHandle(reference); TODO
                        handle = new Handle(reference, obj);
                        handlesLock.EnterWriteLock();
                        try
                        {
                            handlesByUri.Add(reference.Uri, handle);
                        }
                        finally
                        {
                            handlesLock.ExitWriteLock();
                        }
                        return handle;
                    }
                }
                finally
                {
                    if (handlesLock.IsUpgradeableReadLockHeld)
                    {
                        handlesLock.ExitUpgradeableReadLock();
                    }
                }
            }
            else
            {
                return CreateHandle(reference);
            }
        }

        public static IHandle CreateHandle(IReference reference)
        {
            var handle = new Handle(reference);

            //if (ShareHandles && handlesByUri.ContainsKey(reference.Uri)) // TODO: With locking
            //{
            //    handlesByUri.Add(reference.Uri, handle);
            //}
            return handle;
        }
    }

    public static class HandleExtensions
    {
        // TODO: Add more obj params
        public static IHandle ToHandle(this string uri) // Add obj?
        {
            return HandleFactory.GetHandle(uri.ToReference());
        }

        public static IHandle ToHandle(this IReference reference, object obj = null)
        {
            return HandleFactory.GetHandle(reference, obj);
        }
        
#if !AOT
        public static IHandle<T> ToHandle<T>(this IReferencable referencable, T obj = null)
            where T : class//, new()
        {
            return HandleFactory<T>.GetHandle(referencable.Reference, obj);
        }
#endif

        public static IHandle ToHandle(this IReference reference, object obj , Type type)
		{
			// TODO: Get HandleFactory<T> via reflection, perhaps only if not AOT
			return HandleFactory.GetHandle(reference, obj);
		}

        public static IHandle CreateHandle(this IReference reference)
        {
            return HandleFactory.CreateHandle(reference);
        }
#if !AOT
        public static IHandle<T> CreateHandle<T>(this IReference reference)
            where T : class, new()
        {
            return HandleFactory<T>.CreateHandle(reference);
        }
#endif
        public static IHandle ToHandle(this IReferencable referencable)
        {
            return HandleFactory.GetHandle(referencable.Reference);
        }
    }


}
