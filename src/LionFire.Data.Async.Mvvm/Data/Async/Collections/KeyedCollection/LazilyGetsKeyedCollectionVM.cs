using LionFire.Data.Collections;
using LionFire.ExtensionMethods;
using LionFire.Data.Async.Gets;
using System.ComponentModel;
using System.Reactive.Subjects;

namespace LionFire.Data.Mvvm;

public class LazilyGetsKeyedCollectionVM<TKey, TValue, TValueVM, TCollection>
    : LazilyGetsCollectionVM<TValue, TValueVM, TCollection>
    , IGetsKeyedCollectionVM<TKey, TValue, TValueVM, TCollection>
    where TKey : notnull
    where TCollection : IEnumerable<TValue>
{
    public Func<TValue, TKey> KeySelector { get; set; } = v => throw new NotImplementedException();

    #region Lifecycle

    public LazilyGetsKeyedCollectionVM(IViewModelProvider viewModelProvider) : base(viewModelProvider)
    {
        this.WhenAnyValue(vm => vm.Source)
            .Subscribe(OnSourceChanged);

        this
            .WhenAnyValue(vm => vm.PreferredSource!.ObservableCache)
            .Select(observableCache => observableCache
                    .Connect()
                    .Transform(GetViewModel)
            )
            .Subscribe(valueVMCollections);

        ValueVMCollections.Subscribe(oc => oc
            .BindToObservableListAction(list => ValueVMs = list)
            .Subscribe());
    }

    #endregion

    TValueVM GetViewModel(TValue v)
    {
        var vm = ViewModelProvider.Activate<TValueVM, TValue>(v);

        if (vm is INotifyPropertyChanged inpc)
        {
            inpc.PropertyChanged += Inpc_PropertyChanged;
        }
        return vm;
    }

    public IObservable<string> ViewModelPropertyChanges => viewModelPropertyChanges;
    Subject<string> viewModelPropertyChanges = new();
    private void Inpc_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        viewModelPropertyChanges.OnNext(e.PropertyName);
    }

    #region State (derived)

    #region Source

    public IObservableCacheKeyableGetter<TKey, TValue>? PreferredSource // REVIEW type, create and use interface?
    {
        get => base.Source as IObservableCacheKeyableGetter<TKey, TValue>; 
        set => base.Source = value;
    }

    protected void OnSourceChanged(IGetter? newValue)
    {
        ((ReactiveObject)this).RaisePropertyChanged(nameof(PreferredSource));
    }

    #endregion

    public IObservable<IObservable<IChangeSet<TValueVM, TKey>>> ValueVMCollections => valueVMCollections;
    BehaviorSubject<IObservable<IChangeSet<TValueVM, TKey>>> valueVMCollections = new(Observable.Empty<IChangeSet<TValueVM, TKey>>());

    public IObservableList<TValueVM>? ValueVMs
    {
        get => valueVMs;
        set
        {
            if (ReferenceEquals(valueVMs, value)) { return; }
            var old = valueVMs;
            old?.Dispose();
            valueVMs = value;
            Debug.WriteLine("ValueVMs set to list with " + value?.Count + " items");
        }
    }
    private IObservableList<TValueVM>? valueVMs;

    #endregion
}


// Without TCollection
public class LazilyGetsKeyedCollectionVM<TKey, TValue, TValueVM>
    : LazilyGetsKeyedCollectionVM<TKey, TValue, TValueVM, IEnumerable<TValue>>
    where TKey : notnull
{
    #region Lifecycle

    public LazilyGetsKeyedCollectionVM(IViewModelProvider viewModelProvider) : base(viewModelProvider)
    {
    }

    #endregion
}
