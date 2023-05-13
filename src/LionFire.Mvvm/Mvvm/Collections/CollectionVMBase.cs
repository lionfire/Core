#nullable enable

using LionFire.Dependencies;
using LionFire.Structures.Keys;

namespace LionFire.Mvvm;

public abstract class CollectionVMBase<TItem> : ReactiveObject
    where TItem : notnull
{
    #region Dependencies

    #region Optional

    public IViewModelProvider? ViewModelProvider { get; set; } = DependencyContext.Current?.GetService<IViewModelProvider>();

    //public IKeyProvider<TItem, object>? FallbackKeyProvider { get; set; }

    #endregion

    #endregion

    //protected CompositeDisposable compositeDisposable = new();

    #region Lifecycle

    public CollectionVMBase()
    {

    }

    //public virtual void Dispose()
    //{
    //    compositeDisposable.Dispose();
    //}

    #endregion

    #region Items

    #region Items: Model

    public abstract IEnumerable<TItem> Items { get; protected set; } 

    #endregion

    #region Items: ViewModel

    #endregion

    #endregion
}

