using LionFire.Resolves;
using System.Reactive;
using MorseCode.ITask;
using DynamicData;
using LionFire.Collections.Async;
using Newtonsoft.Json.Linq;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using Microsoft.Extensions.DependencyInjection;
using DynamicData.Binding;
using ReactiveUI;
using System.Reactive.Subjects;
using LionFire.ExtensionMethods;
using System.Diagnostics;
using System;

namespace LionFire.Mvvm;

public class LazilyResolvesVM<T> : ResolvesVM<T>, ILazilyResolvesVM<T>
{
    #region Parameters

    public new ILazilyResolves<T>? Source { get => (ILazilyResolves<T>?)base.Source; set => base.Source = value; }

    public bool ShowRefreshIfHasNoValue { get; set; } = true;

    #endregion

    #region Lifecycle

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public LazilyResolvesVM()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
        this.WhenAnyValue(r => r.Source)
            .Subscribe(resolves =>
            {
                ResolveIfNeeded = ReactiveCommand.CreateFromTask<Unit, ILazyResolveResult<T>>(
                    _ => resolves.TryGetValue().AsTask(),
                    canExecute: Observable.Create<bool>(o => { o.OnNext(resolves != null); o.OnCompleted(); return Disposable.Empty; })
                );

                #region NeedsLazilyResolve

                if (resolves is IObservableResolves<T> whenResolves)
                {
                    // Subscribe to the IObservableResolves
                    whenResolves.Resolves.Subscribe(async resultTask =>
                    {
                        var result = await resultTask;
                        HasValue = result.HasValue;
                    });
                }
                else
                {
                    // Subscribe to the ReactiveCommand in this
                    Resolve.Subscribe(result => HasValue = result.HasValue);
                }
                HasValue = Source?.HasValue == true;

                #endregion
            });
    }

    #endregion

    #region ILazilyResolvesVM<T>

    public ReactiveCommand<Unit, ILazyResolveResult<T>> ResolveIfNeeded { get; private set; }

    [Reactive]
    public bool HasValue { get; set; }
    //{ get => !Source.HasValue; } // Not reactive

    #endregion
}
