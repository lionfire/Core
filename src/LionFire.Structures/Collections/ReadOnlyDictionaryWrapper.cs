using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire.Collections
{
    // Based on / Inspired by https://stackoverflow.com/a/13602918/208304

    public class ReadOnlyDictionaryWrapper<TKey, TValue, TReadOnlyValue> : IReadOnlyDictionary<TKey, TReadOnlyValue> 
        where TValue : TReadOnlyValue
    {
        private IDictionary<TKey, TValue> _dictionary;

        public ReadOnlyDictionaryWrapper(IDictionary<TKey, TValue> dictionary)
        {
            if (dictionary == null) throw new ArgumentNullException("dictionary");
            _dictionary = dictionary;
        }

        public IEnumerable<TKey> Keys => _dictionary.Keys;
        public IEnumerable<TReadOnlyValue> Values => _dictionary.Values.Cast<TReadOnlyValue>();

        public TReadOnlyValue this[TKey key] => _dictionary[key];
        public bool ContainsKey(TKey key) => _dictionary.ContainsKey(key);

        public bool TryGetValue(TKey key, out TReadOnlyValue value)
        {
            var result = _dictionary.TryGetValue(key, out TValue v);
            value = v;
            return result;
        }

        public int Count => _dictionary.Count;

        public IEnumerator<KeyValuePair<TKey, TReadOnlyValue>> GetEnumerator()
            => _dictionary.Select(x => new KeyValuePair<TKey, TReadOnlyValue>(x.Key, x.Value)).GetEnumerator();
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}
