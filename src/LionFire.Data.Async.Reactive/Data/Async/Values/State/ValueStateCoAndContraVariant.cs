using LionFire.Data.Async.Sets;
using LionFire.IO;
using LionFire.Reactive;
using System.Reactive.Disposables;

namespace LionFire.Data.Async;

public class ValueStateCoAndContraVariant<TValueReal, TValueExposed> : ReactiveObject, IValueState<TValueExposed>, IDisposable
{
    public IValueState<TValueReal> Wrapped { get; }

    public ValueStateCoAndContraVariant(IValueState<TValueReal> wrapped)
    {
        Wrapped = wrapped;

        d =
        [
            wrapped.WhenAnyValue(x => x.Value).Subscribe(x => this.RaisePropertyChanged(nameof(Value))),
            wrapped.WhenAnyValue(x => x.StateFlags).Subscribe(x => this.RaisePropertyChanged(nameof(StateFlags))),
        ];
    }

    public void Dispose()
    {
        d.Dispose();
    }
    CompositeDisposable d;

    #region IValueState<TValueExposed>

    public TValueExposed? Value { get => (TValueExposed?)(object?)Wrapped.Value; set => Wrapped.Value = (TValueReal?)(object?)value; }

    #region Pass-thru

    public ValueStateFlags StateFlags => Wrapped.StateFlags;

    public IODirection IODirection => Wrapped.IODirection;

    public void DiscardStagedValue() => Wrapped.DiscardStagedValue();

    public Task<ISetResult> Set(CancellationToken cancellationToken = default) => Wrapped.Set(cancellationToken);


    #endregion

    #endregion
}

