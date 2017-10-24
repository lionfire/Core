using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace LionFire.MultiTyping
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// If you want HasObject true with Object equal to null, you must use another implementation that allows for this scenario.
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    public class LazyIs<T> : Has<T>
        where T : class, new()
    {
        public override T Object
        {
            get
            {
                if (!HasObject) { GetOrUpdate(() => new T()); }
                return base.Object;
            }
        }
    }

    public class LazyIsFactory<T> : Has<T>
       where T : class, new()
    {
        public override T Object
        {
            get
            {
                if (!HasObject) { GetOrUpdate(() => Factory()); }
                return base.Object;
            }
        }
        public Func<T> Factory { get; set; }
    }

    public class Has<T> : IDisposable 
        where T : class
    {
        public virtual T Object => _object;
        private T _object;
        public bool HasObject => Object != default(T);

        /// <summary>
        /// Threadsafe.  May call the createMethod unnecessarily.
        /// </summary>
        /// <returns></returns>
        public bool GetOrUpdate(Func<T> updateMethod)
        {
            if (_object != default(T)) return false;
            var newObj = updateMethod();
            return Interlocked.CompareExchange<T>(ref _object, newObj, default(T)) == default(T);
        }

        public void Dispose()
        {
            _object = default(T);
        }
    }


#if Example // TOTEST
    public class X { }
    public class HasExample : IIs<X>, IIsLateCreatable<T>
    {
        Is<X> impl = new Is<X>();
        LazyIs<X> impl2 = new LazyIs<X>();
        LazyIsFactory<X> impl3 = new LazyIsFactory<X>();

        public X Object => impl.Object;

        public bool HasObject => impl.HasObject;
    }
#endif
}
