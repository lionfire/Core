using LionFire.Data.Async;
using LionFire.Data.Async.Gets;
using System.ComponentModel;

namespace LionFire.Data.Mvvm;


public class GetterToRxO<TValue>
    : ReactiveObject
    , IGetterRxO<TValue>
    , IDisposable
    , IMightFireChangeEvents
{
    #region Relationships

    IGetter<TValue> source;

    #endregion

    public bool FiresChangeEvents => false;

    #region Lifecycle

    public GetterToRxO(IGetter<TValue> getter)
    {
        source = getter;

        if (getter is INotifyPropertyChanged inpc) { inpc.PropertyChanged += Inpc_PropertyChanged; RaisesPropertyChanged = true; }
        if (getter is INotifyPropertyChanging inpc2) { inpc2.PropertyChanging += Inpc_PropertyChanging; RaisesPropertyChanging = true; }
    }

    public void Dispose()
    {
        if (source is INotifyPropertyChanged inpc) { inpc.PropertyChanged -= Inpc_PropertyChanged; }
        if (source is INotifyPropertyChanging inpc2) { inpc2.PropertyChanging -= Inpc_PropertyChanging; }
        source = null;
    }

    #endregion

    public bool RaisesPropertyChanged { get; }
    public bool RaisesPropertyChanging { get; }

    #region Adapter: Change events

    private void Inpc_PropertyChanged(object? sender, PropertyChangedEventArgs e) => this.RaisePropertyChanged(e.PropertyName);
    private void Inpc_PropertyChanging(object? sender, PropertyChangingEventArgs e) => this.RaisePropertyChanging(e.PropertyName);

    #endregion

    #region Pass-thru

    public TValue? ReadCacheValue => source.ReadCacheValue;

    public TValue? Value => source.Value;

    public bool HasValue => source.HasValue;

    public void Discard() => source.Discard();

    public void DiscardValue() => source.DiscardValue();

    public ITask<IGetResult<TValue>> Get(CancellationToken cancellationToken = default) => source.Get(cancellationToken);

    public ITask<IGetResult<TValue>> GetIfNeeded() => source.GetIfNeeded();

    public IGetResult<TValue> QueryGetResult() => source.QueryGetResult();

    public IObservable<IGetResult<TValue>> GetResults => source.GetResults;

    public IObservable<ITask<IGetResult<TValue>>> GetOperations => source.GetOperations;

    #endregion
}
