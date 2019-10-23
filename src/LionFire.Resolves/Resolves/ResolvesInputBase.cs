using LionFire.Structures;
using System;

namespace LionFire.Resolves
{
    public abstract class ResolvesInputBase<TKey> : IDisposable, IKeyed<TKey>
    {
        protected ResolvesInputBase() { }
        protected ResolvesInputBase(TKey key) { this.Key = key; }

        #region Input

        [SetOnce]
        public TKey Key
        {
            get => isDisposed ? throw new ObjectDisposedException(nameof(ResolvesInputBase<TKey>)) : key;
            set
            {
                if (ReferenceEquals(key, value)) return;
                if (key != default) throw new AlreadySetException();
                key = value;
            }
        }
        protected TKey key;

        public virtual void Dispose()
        {
            // TODO: THREADSAFETY
            isDisposed = true;
            key = default;
        }
        protected bool isDisposed;

        #endregion
    }
}
