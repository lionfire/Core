using LionFire.Reactive.Persistence;
using LionFire.Structures;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using System.Reactive.Linq;

namespace LionFire.Mvvm; // REVIEW: Should be LionFire.Reactive.Persistence.Mvvm?



public partial class ObservableReaderVM<TKey, TValue, TValueVM> : ReactiveObject
where TKey : notnull
where TValue : notnull
{
    #region Parameters

    public bool AutoLoadAll
    {
        get => autoLoadAll;
        set
        {
            if (autoLoadAll == value) return;
            autoLoadAll = value;

            autoLoadAllSubscription?.Dispose();
            if (value)
            {
                autoLoadAllSubscription = Data.ListenAllKeys();
            }
        }
    }
    private IDisposable? autoLoadAllSubscription;

    public IObservableReader<TKey, TValue> Data { get; }

    private bool autoLoadAll = true;

    #endregion

    #region Lifecycle

    public ObservableReaderVM(IObservableReader<TKey, TValue> data)
    {
        Data = data;
        //data.ListenAllKeys();


    }

    #endregion
}

public partial class ObservableReaderItemVM<TKey, TValue, TValueVM> : ReactiveObject
    where TKey : notnull
    where TValue : notnull
{
    public IObservableReader<TKey, TValue> Data { get; }

    public ObservableReaderItemVM(IObservableReader<TKey, TValue> data)
    {
        Data = data;

        this.WhenAnyValue(x => x.Id).DistinctUntilChanged().Subscribe(id =>
        {
            listenSubscription?.Dispose();
            Value = default;

            if (id != null)
            {
                IsLoading = true;
                var valueObservable  = data.GetValueObservableIfExists(id);
                if (valueObservable != null)
                {
                    listenSubscription = valueObservable.Subscribe(v =>
                    {
                        Value = v;
                        IsLoading = false;
                    });
                } else
                {
                    Debug.WriteLine("TODO: Why doesn't valueObservable exist?");
                    IsLoading = false;
                }
            }
        });
    }
    IDisposable? listenSubscription;

    #region State

    [Reactive]
    private TKey? _id;

    [Reactive]
    private TValue? _value;
    //TValue? IReadWrapper<TValue>.Value => Value;

    [Reactive]
    private bool _isLoading;

    #endregion
}
