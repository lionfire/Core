// Based on https://github.com/artrey/ConcurrentObservableCollections/commit/bfa26314f3fe8dfd587fb3298feb608ef2a701be
// MIT License - Copyright (c) 2018 artrey

using System.Collections.Specialized;

namespace LionFire.Structures
{
    public class DictionaryChangedEventArgs<TKey, TValue>
    {
        public DictionaryChangedEventArgs(NotifyCollectionChangedAction action)
        {
            Action = action;
        }

        public DictionaryChangedEventArgs(NotifyCollectionChangedAction action, TKey key, TValue newValue, TValue oldValue)
            : this(action)
        {
            Key = key;
            NewValue = newValue;
            OldValue = oldValue;
        }

        public NotifyCollectionChangedAction Action { get; }
        public TKey Key { get; }
        public TValue NewValue { get; }
        public TValue OldValue { get; }
    }
}
