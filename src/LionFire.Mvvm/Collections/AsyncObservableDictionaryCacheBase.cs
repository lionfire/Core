using CommunityToolkit.Mvvm.ComponentModel;
using ObservableCollections;
using System.Collections;

namespace LionFire.Mvvm;

//public class AsyncObservableDictionaryVM<TKey, TValue>: AsyncObservableDictionaryVMBase<TKey, TValue>
//{
//    public IAsyncDictionary<TKey, TValue> Source { get; set; }

//}

public class AsyncObservableDictionaryCacheBase<TKey, TValue> : ObservableObject, IAsyncCollectionCache<KeyValuePair<TKey, TValue>>
    where TKey : notnull
{

    #region State

    public virtual ObservableDictionary<TKey, TValue>? Dictionary
    {
        get => collection;
        set
        {
            if (ReferenceEquals(value, collection)) { return; }
            if (collection != null) { throw new AlreadySetException(); }
            collection = value is ObservableDictionary<TKey, TValue> collectionValue
                ? collectionValue
                : throw new ArgumentException($"{nameof(value)} must be of type {typeof(ObservableDictionary<TKey, TValue>).FullName}");
        }
    }
    protected ObservableDictionary<TKey, TValue>? collection;
    IObservableCollection<KeyValuePair<TKey, TValue>>? IAsyncCollectionCache<KeyValuePair<TKey, TValue>>.Collection => Dictionary;

    #endregion

    #region Collection-related

    public int Count => throw new NotImplementedException();
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => collection?.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

    public virtual bool IsReadOnly => true;

    #endregion

    #region Retrieve

    public virtual bool HasRetrieved => false;

    public virtual bool IsRetrieving => false;

    public virtual bool CanRetrieve => false;

    public virtual Task<IEnumerable<KeyValuePair<TKey, TValue>>> Retrieve(bool syncToCollection = true, CancellationToken cancellationToken = default) => throw new NotSupportedException();

    #endregion

    #region Add

    public IEnumerable<Type>? CreateableTypes => Enumerable.Empty<Type>();
    public IEnumerable<KeyValuePair<TKey, TValue>> Adding => Enumerable.Empty<KeyValuePair<TKey, TValue>>();

    public Task Add(KeyValuePair<TKey, TValue> item)
    {
        throw new NotSupportedException();
    }

    public Task AddRange(IEnumerable<KeyValuePair<TKey, TValue>> item)
    {
        throw new NotSupportedException();
    }

    #endregion

    #region Remove

    public IEnumerable<KeyValuePair<TKey, TValue>> Removing => Enumerable.Empty<KeyValuePair<TKey, TValue>>();
    public Task<bool> Remove(KeyValuePair<TKey, TValue> item)
    {
        throw new NotSupportedException();
    }

    #endregion

    public bool CanAdd => false;
        public bool CanCreate => false;
    public virtual Task<KeyValuePair<TKey, TValue>> Create(Type type, params object[]? constructorParameters) => throw new NotSupportedException();

    public bool CanAddNew => false;
    public virtual Task<KeyValuePair<TKey, TValue>> AddNew(Type type, params object[]? constructorParameters) => throw new NotSupportedException();

}
