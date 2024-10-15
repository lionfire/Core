using LionFire.Data.Collections;
using LionFire.ExtensionMethods;
using LionFire.Data.Async.Gets;
using System.ComponentModel;
using System.Reactive.Subjects;
using DynamicData;
using DynamicData.Binding;
using System.Collections.ObjectModel;

namespace LionFire.Data.Mvvm;

/// <summary>
/// How to use:
/// - Set Source, which has a property named ObservableCache
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TValue"></typeparam>
/// <typeparam name="TValueVM"></typeparam>
/// <typeparam name="TCollection"></typeparam>
public class LazilyGetsKeyedCollectionVM<TKey, TValue, TValueVM, TCollection>
    : LazilyGetsCollectionVM<TValue, TValueVM, TCollection>
    , IGetsKeyedCollectionVM<TKey, TValue, TValueVM, TCollection>
    where TKey : notnull
    where TCollection : IEnumerable<TValue>
    where TValue : notnull
    where TValueVM : notnull//, IKeyed<TKey>
{
    public Func<TValueVM, TKey> KeySelector { get; set; } 
    //public bool IsObservable { get; protected set; } // TODO: Move to base and implement IObservableList

    #region Lifecycle

    public LazilyGetsKeyedCollectionVM(IViewModelProvider viewModelProvider, Func<TValueVM, TKey>? keySelector = null) : base(viewModelProvider)
    {
        KeySelector = keySelector;
        if(KeySelector == null)
        {
            if (typeof(TValueVM).IsAssignableTo(typeof(IKeyed<TKey>)))
            {
                KeySelector = vm => ((IKeyed<TKey>)vm).Key;
            }
            if (typeof(TValueVM).IsAssignableTo(typeof(IViewModel<TKey>)))
            {
                KeySelector = vm => ((IViewModel<TKey>)vm).Value!;
            }
        }
        if (KeySelector == null) { KeySelectors<TValueVM, TKey>.GetKeySelector(); }

        this.WhenAnyValue(vm => vm.Source)
            .Subscribe(OnSourceChanged);

#if true
        this
            .WhenAnyValue(vm => vm.PreferredSource!.ObservableCache)
            .Select(observableCache => observableCache
                    .Connect()
                    .Transform(CreateViewModel)
            )
        .Switch()
            .BindToObservableListAction(list => ValueVMs = list)
            .Subscribe();
#else
        this
            .WhenAnyValue(vm => vm.PreferredSource!.ObservableCache)
            .Select(observableCache => observableCache
                    .Connect()
                    .Transform(CreateViewModel)
            )
            .Subscribe(valueVMCollections);

        ValueVMCollections.Subscribe(oc => oc
            .BindToObservableListAction(list => ValueVMs = list)
            .Subscribe());
#endif

        observableCache = bindingList
            .AsObservableChangeSet(vm => KeySelector(vm))
            .AsObservableCache();
    }

#endregion

    TValueVM CreateViewModel(TValue v)
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
    
    // REVIEW - is Source needed?  Or superfluous to ValueVMs?

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
            bindingListSubscription?.Dispose();
            bindingListSubscription = null;

            valueVMs = value;

            Debug.WriteLine("ValueVMs set to list with " + value?.Count + " items");

            if (valueVMs != null)
            {
                bindingListSubscription = valueVMs
                    .Connect()
                    .Bind(bindingList)
                    .Subscribe();

           
                    
            }
        }
    }
    private IObservableList<TValueVM>? valueVMs;

    #region IObservableCache

    public  IObservableCache<TValueVM, TKey> ObservableCache => observableCache;
    private readonly IObservableCache<TValueVM, TKey> observableCache;
    private readonly ObservableCollectionExtended<TValueVM> bindingList = new();
    private IDisposable? bindingListSubscription;

    #endregion


    #endregion
}


// Without TCollection
public class LazilyGetsKeyedCollectionVM<TKey, TValue, TValueVM>
    : LazilyGetsKeyedCollectionVM<TKey, TValue, TValueVM, IEnumerable<TValue>>
    where TKey : notnull
    where TValue : notnull
    where TValueVM : notnull//, IKeyed<TKey>
{
    #region Lifecycle

    public LazilyGetsKeyedCollectionVM(IViewModelProvider viewModelProvider) : base(viewModelProvider)
    {
    }

    #endregion
}
