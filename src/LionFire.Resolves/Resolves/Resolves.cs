#nullable enable
using LionFire.Structures;
using Microsoft.Extensions.Logging;
using MorseCode.ITask;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace LionFire.Resolves;

// REVIEW - Is this still needed with covariant ITask?
//public abstract class Resolves<TKey, TValue, TValueReturned> : DisposableKeyed<TKey>
//    where TValueReturned : TValue

//public class SmartWrappedValue<TValue>
//{
//    public TValue ProtectedValue
//    {
//        get => protectedValue;
//        set
//        {
//            if (EqualityComparer<TValue>.Default.Equals(protectedValue, value)) return;
//            var oldValue = protectedValue;

//            ValueChangedPropagation.Detach(protectedValue);
//            protectedValue = value;
//            WrappedValueForFromTo?.Invoke(this, oldValue, protectedValue);
//            ValueChangedPropagation.Attach(protectedValue, o => WrappedValueChanged?.Invoke(this));

//            WrappedValueChanged?.Invoke(this); // Assume that there was a change

//            OnValueChanged(value, oldValue);
//        }
//    }
//    /// <summary>
//    /// Raw field for protectedValue.  Should typically call OnValueChanged(TValue newValue, TValue oldValue) after this field changes.
//    /// </summary>
//    protected TValue protectedValue;

//    public event Action<INotifyWrappedValueReplaced, object, object> WrappedValueForFromTo;
//    public event Action<INotifyWrappedValueChanged> WrappedValueChanged;
//}

/// <summary>
/// 
/// </summary>
/// <remarks>
/// Only requires one method to be implemented: ResolveImpl.
/// </remarks>
public abstract class Resolves<TKey, TValue> : DisposableKeyed<TKey>, INotifyWrappedValueChanged, INotifyWrappedValueReplaced
    // REVIEW Inherit ILazilyResolves<TValue> on concrete classes, as desired?
    //  - Or rename this class to LazilyResolvesDeluxe<> to distinguish from LazilyResolves<>?
    //  - , ILazilyResolves<TValue> 
{
    static ValueChangedPropagation ValueChangedPropagation = new ValueChangedPropagation();

    #region Configuration

    public static bool DisposeValue { get; set; } = true;

    #endregion

    #region Construction

    protected Resolves() { }
    protected Resolves(TKey input) : base(input) { }

    ///// <summary> // Is this needed/helpful?
    ///// Do not use this in derived classes that are purely resolve-only and not intended to set an initial value.
    ///// </summary>
    ///// <param name="input"></param>
    ///// <param name="initialValue"></param>
    //protected Resolves(TKey input, TValue initialValue) : base(input)
    //{
    //    if (initialValue != default)
    //    {
    //        ProtectedValue = initialValue;
    //    }
    //}

    #endregion

    #region Value

    /// <summary>
    /// True if internal Value field is not default.  If default is a valid value, use DefaultableValue&lt;TValue&gt; as TValue type
    /// </summary>
    public bool HasValue => !EqualityComparer<TValue>.Default.Equals(ProtectedValue, default);

    public TValue Value
    {
        [Blocking(Alternative = nameof(TryGetValue))]
        get => ProtectedValue ?? TryGetValue().Result.Value;
    }

    //SmartWrappedValue SmartWrappedValue = new SmartWrappedValue();
    //protected TValue ProtectedValue { get=>SmartWrappedValue.Prote}

    // REVIEW: should this be TValue?  Should TValue have constraint of : default?
    protected TValue ProtectedValue
    {
        get => protectedValue;
        set
        {
            if (EqualityComparer<TValue>.Default.Equals(protectedValue, value)) return;
            var oldValue = protectedValue;

            if (DisposeValue && oldValue is IDisposable d) {
                Log.Get<Resolves<TKey, TValue>>().LogDebug("Disposing object of type {Type}", d.GetType().FullName);
                d.Dispose(); 
            }

            ValueChangedPropagation.Detach(this, protectedValue);
            protectedValue = value;
            ValueChangedPropagation.Attach(this, protectedValue, o => WrappedValueChanged?.Invoke(this));
            WrappedValueForFromTo?.Invoke(this, oldValue, protectedValue);
            WrappedValueChanged?.Invoke(this); // Assume that there was a change

            OnValueChanged(value, oldValue);
        }
    }
    /// <summary>
    /// Raw field for protectedValue.  Should typically call OnValueChanged(TValue newValue, TValue oldValue) after this field changes.
    /// </summary>
    protected TValue protectedValue;

    public event Action<INotifyWrappedValueReplaced, object, object> WrappedValueForFromTo;
    public event Action<INotifyWrappedValueChanged> WrappedValueChanged;

    /// <summary>
    /// Raised when ProtectedValue changes
    /// </summary>
    /// <param name="newValue"></param>
    /// <param name="oldValue"></param>
    protected virtual void OnValueChanged(TValue newValue, TValue oldValue) { }

    #endregion

    #region GetValue

    public async ITask<ILazyResolveResult<TValue>> TryGetValue()
    {
        var currentValue = ProtectedValue;
        if (!EqualityComparer<TValue>.Default.Equals(currentValue, default)) return new ResolveResultNoop<TValue>(ProtectedValue);

        var resolveResult = await Resolve().ConfigureAwait(false);
        return new LazyResolveResult<TValue>(resolveResult.HasValue, resolveResult.Value);
    }

    #endregion

    #region QueryValue

    public ILazyResolveResult<TValue> QueryValue()
    {
        var currentValue = ProtectedValue;
        return !EqualityComparer<TValue>.Default.Equals(currentValue, default) ? new ResolveResultNoop<TValue>(ProtectedValue) : (ILazyResolveResult<TValue>)ResolveResultNotResolved<TValue>.Instance;
    }

    #endregion

    #region Discard

    public virtual void DiscardValue() => ProtectedValue = default;

    #endregion

    #region Resolve

    public async ITask<IResolveResult<TValue>> Resolve()
    {
        var resolveResult = await ResolveImpl();
        Debug.Assert(resolveResult != null, "ResolveImpl must not return null");
        ProtectedValue = resolveResult.Value;
        return resolveResult;
    }

    #endregion

    #region Abstract

    protected abstract ITask<IResolveResult<TValue>> ResolveImpl();

    #endregion

    #region OLD

    //public async ITask<ILazyResolveResult<TValueReturned>> GetValue2()
    //{
    //    var currentValue = ProtectedValue;
    //    if (currentValue != null) return new LazyResolveResultNoop<TValueReturned>((TValueReturned)(object)ProtectedValue);

    //    var resolveResult = await Resolve();
    //    return new LazyResolveResult<TValueReturned>(resolveResult.HasValue, (TValueReturned)(object)resolveResult.Value);
    //}

    #endregion

    public override void Dispose()
    {
        base.Dispose();
        DiscardValue(); // Disposes ProtectedValue, if any
    }
}

