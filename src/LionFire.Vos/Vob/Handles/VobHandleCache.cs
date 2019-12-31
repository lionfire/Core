using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Vos.Handles
{
#if TODO // Probably.  Not exactly sure how
    public class VobHandleCache
    {

        public class HandleCache
        {

        }
        /// <seealso cref="CreateHandle(Type)"/>
        //public VobHandle<T> GetHandle<T>() => (VobHandle<T>)handles.GetOrAdd(typeof(T), t => CreateHandle(t));
        public IVobHandle GetHandle(Type type) => (IVobHandle)handles.GetOrAdd(type, t => CreateHandle(t));

        public IVobHandle CreateHandle(Type type)
        {
            Type vhType = typeof(VobHandle<>).MakeGenericType(type);
            return (IVobHandle)Activator.CreateInstance(vhType, this);
        }
        internal IVobReadHandle CreateReadHandle(Type type)
        {
            Type vhType = typeof(VobReadHandle<>).MakeGenericType(type);
            return (IVobReadHandle)Activator.CreateInstance(vhType, this);
        }

        private ConcurrentDictionary<Type, object> handles = new ConcurrentDictionary<Type, object>();
        private readonly ConcurrentDictionary<Type, object> readHandles = new ConcurrentDictionary<Type, object>();


    }
#endif
}
