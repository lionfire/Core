using System;
using System.Threading.Tasks;

namespace LionFire.Handles
{
    public class NullReadHandle<T> : IReadHandle<T>
    {

        public T Object => default(T);
        public bool HasObject => false;

        public string Key => null;

        public event Action<IReadHandle<T>, T, T> ObjectChanged { add { } remove { } }

        public void ForgetObject()
        {
        }

        public Task<bool> TryResolveObject(object persistenceContext = null)
        {
            return Task.FromResult(true);
        }
    }

}
