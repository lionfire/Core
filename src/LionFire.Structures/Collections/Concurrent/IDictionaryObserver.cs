// Based on https://github.com/artrey/ConcurrentObservableCollections/commit/bfa26314f3fe8dfd587fb3298feb608ef2a701be
// MIT License - Copyright (c) 2018 artrey

namespace LionFire.Structures
{
    public interface IDictionaryObserver<TKey, TValue>
    {
        void OnEventOccur(DictionaryChangedEventArgs<TKey, TValue> args);
    }
}
