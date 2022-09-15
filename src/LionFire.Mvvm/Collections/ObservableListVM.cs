using ObservableCollections;

namespace LionFire.Mvvm;

public class ObservableListVM<T> : AsyncObservableCollectionVMBase<T, ObservableList<T>>, ICollectionVM<T>
{
    public ObservableListVM() { }
    public ObservableListVM(IObservableCollection<T> collection) : base(collection) { }
}
