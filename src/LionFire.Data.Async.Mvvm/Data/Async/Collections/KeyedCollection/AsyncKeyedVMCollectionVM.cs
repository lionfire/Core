namespace LionFire.Data.Mvvm;

public class AsyncKeyedVMCollectionVM<TKey, TValue, TValueVM>
    : AsyncKeyedXCollectionVMBase<TKey, TValue, TValueVM>
    where TKey : notnull
    where TValue : notnull
    where TValueVM : notnull// , IKeyed<TKey> - Not sure I want to impose this constraint
    // , IViewModel<TValue> // Suggested only
{
    #region Lifecycle

    public AsyncKeyedVMCollectionVM(IViewModelProvider viewModelProvider) : base(viewModelProvider)
    {
        throw new NotImplementedException();
    }

    #endregion
}
