// Based on https://github.com/artrey/ConcurrentObservableCollections/commit/bfa26314f3fe8dfd587fb3298feb608ef2a701be
// MIT License - Copyright (c) 2018 artrey

using System;

namespace LionFire.Structures
{
    internal class ActionDictionaryObserver<TKey, TValue> : IDictionaryObserver<TKey, TValue>
    {
        private readonly Action<DictionaryChangedEventArgs<TKey, TValue>> _action;

        public ActionDictionaryObserver(Action<DictionaryChangedEventArgs<TKey, TValue>> action)
        {
            _action = action;
        }

        public void OnEventOccur(DictionaryChangedEventArgs<TKey, TValue> args)
        {
            _action.Invoke(args);
        }
    }
}
