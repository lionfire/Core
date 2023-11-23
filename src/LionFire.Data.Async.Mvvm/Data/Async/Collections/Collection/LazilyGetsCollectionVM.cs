using LionFire.Data.Async.Gets;
using LionFire.Mvvm;

namespace LionFire.Data.Mvvm;

// Without TCollection
public class LazilyGetsCollectionVM<TValue, TValueVM>
    : LazilyGetsCollectionVM<TValue, TValueVM, IEnumerable<TValue>>
{
    #region Lifecycle

    public LazilyGetsCollectionVM(IViewModelProvider viewModelProvider) : base(viewModelProvider)
    {
    }

    #endregion
}

public class LazilyGetsCollectionVM<TValue, TValueVM, TCollection>
    : GetterVM<TCollection>
    , IGetsCollectionVM<TValue, TValueVM, TCollection>
    where TCollection : IEnumerable<TValue>
{
    #region Dependencies

    public IViewModelProvider ViewModelProvider { get; }

    #endregion

    //IStatelessGetter<TCollection>? IReadWrapper<IStatelessGetter<TCollection>>.Value => Source;>


    #region Lifecycle

    public LazilyGetsCollectionVM(IViewModelProvider viewModelProvider)
    {
        ViewModelProvider = viewModelProvider;
    }

    #endregion
}