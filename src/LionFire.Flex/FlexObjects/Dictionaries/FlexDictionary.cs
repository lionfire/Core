//using LionFire.ExtensionMethods;
using System;
using System.Collections.Concurrent;

namespace LionFire.FlexObjects
{

    /// <summary>
    /// A dictionary of IFlex objects
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public class FlexDictionary<TKey> : IHasFlexDictionary<TKey>
    {
        public ConcurrentDictionary<TKey, IFlex> Values { get; } = new ConcurrentDictionary<TKey, IFlex>();

        FlexDictionary<TKey> IHasFlexDictionary<TKey>.FlexDictionary => this;

        private Func<TKey, IFlex> factory = _ => new FlexObject();

        public IFlex GetFlex(TKey key) => Values.GetOrAdd(key, factory);
        public IFlex QueryFlex(TKey key)
        {
            if (Values.TryGetValue(key, out IFlex existing)) return existing;
            return null;
        }

        public void Add<T>(TKey key, T value)
        {
            GetFlex(key).Add<T>(value);  // REVIEW - Add probably shouldn't map to AddType
        }
    }
}

