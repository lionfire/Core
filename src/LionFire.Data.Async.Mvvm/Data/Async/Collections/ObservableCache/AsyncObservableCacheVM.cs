using LionFire.Data.Async.Sets;

namespace LionFire.Data.Mvvm;

//public class AsyncKeyedVMCollectionVM<TKey, TValue, TValueVM>
//    : AsyncObservableXCacheVMBase<TKey, TValue, TValueVM>
//    where TKey : notnull
//    where TValue : notnull
//    where TValueVM : notnull//, IKeyed<TKey>
//    // , IViewModel<TValue> // Suggested only
//{
//    #region Lifecycle

//    public AsyncKeyedVMCollectionVM(IViewModelProvider viewModelProvider) : base(viewModelProvider)
//    {
//        throw new NotImplementedException();
//    }

//    #endregion
//}

public class AsyncObservableCacheVM<TKey, TValue, TValueVM>
    : AsyncObservableXCacheVMBase<TKey, TValue, TValueVM>
    , ICreatesAsyncVM<TValue>
    //, IActivatableViewModel
    where TKey : notnull
    where TValue : notnull
    where TValueVM : notnull//, IKeyed<TKey>
    // , IViewModel<TValue> // Suggested only
{
    public override ICreatesAsync<TValue>? EffectiveCreator => Creator ?? Source as ICreatesAsync<TValue>;


    #region Lifecycle

    public AsyncObservableCacheVM(IViewModelProvider viewModelProvider) : base(viewModelProvider)
    {
    }

    #endregion

    //#region IObservableCreatesAsync<TValue>

    //public abstract IObservable<(Type, object[], Task<KeyValuePair<TKey, TValue>>)> Creates { get; }

    //public abstract Task<KeyValuePair<TKey, TValue>> Create(Type type, params object[] constructorParameters);

    //#endregion
}

