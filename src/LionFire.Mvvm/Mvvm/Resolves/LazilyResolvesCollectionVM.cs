namespace LionFire.Mvvm;

// Without TCollection
public class LazilyResolvesCollectionVM<TValue, TValueVM>
    : LazilyResolvesCollectionVM<TValue, TValueVM, IEnumerable<TValue>>
{
    #region Lifecycle

    public LazilyResolvesCollectionVM(IViewModelProvider viewModelProvider) : base(viewModelProvider)
    {
    }

    #endregion
}

public class LazilyResolvesCollectionVM<TValue, TValueVM, TCollection>
    : LazilyResolvesVM<TCollection>
    , IResolvesCollectionVM<TValue, TValueVM, TCollection>
    where TCollection : IEnumerable<TValue>
{
    #region Dependencies

    public IViewModelProvider ViewModelProvider { get; }

    #endregion

    #region Lifecycle

    public LazilyResolvesCollectionVM(IViewModelProvider viewModelProvider)
    {
        ViewModelProvider = viewModelProvider;
    }

    #endregion
}