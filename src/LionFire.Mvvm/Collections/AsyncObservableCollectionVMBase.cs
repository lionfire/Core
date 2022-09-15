using CommunityToolkit.Mvvm.ComponentModel;
using ObservableCollections;
using System.Collections;

namespace LionFire.Mvvm;

public abstract partial class AsyncObservableCollectionVMBase<T, TCollection> : ObservableObject, ICollectionVM<T>
    where TCollection : IObservableCollection<T>
{
    #region Parameters

    public virtual IEnumerable<Type>? CreateableTypes { get => createableTypes ??= DefaultCreateableTypes; set => createableTypes = value; }
    private IEnumerable<Type>? createableTypes;

    #region Defaults

    public static IEnumerable<Type> DefaultCreateableTypes
    {
        get
        {
            // TODO: use Type introspection service to get list types that derive from TItem
            // alternate: inspect T for attributes
            return Enumerable.Empty<Type>();
        }
    }

    #endregion

    #endregion

    #region Lifecycle

    public AsyncObservableCollectionVMBase() { }
    public AsyncObservableCollectionVMBase(IObservableCollection<T> collection)
    {
        Collection = collection;
    }

    #endregion

    #region State

    public virtual IObservableCollection<T>? Collection
    {
        get => collection;
        set
        {
            if (ReferenceEquals(value, collection)) { return; }
            if (collection != null) { throw new AlreadySetException(); }
            collection = value is TCollection collectionValue
                ? collectionValue
                : throw new ArgumentException($"{nameof(value)} must be of type {typeof(TCollection).FullName}");

        }
    }
    protected TCollection? collection;

    #endregion

    #region Collection-related

    public int Count => collection?.Count ?? 0;

    public IEnumerator<T> GetEnumerator()
        => collection?.GetEnumerator() ?? Enumerable.Empty<T>().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

    public virtual bool IsReadOnly => true;

    #endregion

    #region Retrieve

    public bool HasRetrieved { get => hasRetrieved; protected set => SetProperty(ref hasRetrieved, value); }
    private bool hasRetrieved;

    public bool IsRetrieving { get => isRetrieving; protected set => SetProperty(ref isRetrieving, value); }
    private bool isRetrieving;

    public virtual bool CanRetrieve => false;

    public virtual Task<IEnumerable<T>> Retrieve(CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    #endregion

    #region Add

    public virtual IEnumerable<T> Adding => Enumerable.Empty<T>();

    public virtual Task Add(T item) => throw new NotSupportedException();

    public virtual Task AddRange(IEnumerable<T> items) => Task.WhenAll(items.Select(item => Add(item)));

    #endregion

    #region Remove

    public IEnumerable<T> Removing => Enumerable.Empty<T>();

    public virtual Task<bool> Remove(T item) => throw new NotSupportedException();

    #endregion
}
