using LionFire.Data.Async.Gets;
using LionFire.Results;

namespace LionFire.Data.Async.Sets;

/// <summary>
/// Write-only wrapper around a value that is staged in this object and committed asynchronously.
/// 
/// Only requires one method to be implemented: SetImpl.
/// 
/// If setting to null or default is a legitimate operation, use DefaultableValue&lt;TValue&gt;
/// </summary>
/// <seealso cref="AsyncValue"/>
/// <remarks>
/// TODO: Move this to .Extras DLL, and make AsyncSets in the .Reactive DLL, as a ReactiveObject.
/// </remarks>
public abstract class SetterSlim<TKey, TValue>
    : DisposableKeyed<TKey>
    , IDiscardableValue
    // TODO: Add more interfaces
    where TKey : class
    where TValue : class
{
    #region Construction

    protected SetterSlim() { }
    protected SetterSlim(TKey input) : base(input) { }

    #endregion

    #region Value

    /// <summary>
    /// For nullable values, use TValue of DefaultableValue&lt;TValue&gt;
    /// </summary>
    public bool HasValue => ProtectedValue != default;

    public TValue Value => ProtectedValue;

    protected TValue ProtectedValue // TODO: Change this to StagedValue
    {
        get => protectedValue;
        set
        {
            if (System.Collections.Generic.Comparer<TValue>.Default.Compare(protectedValue, value) == 0) return;
            var oldValue = protectedValue;
            protectedValue = value;
            OnValueChanged(value, oldValue);
        }
    }
    private TValue? protectedValue;

    #endregion

    #region Discard

    public virtual void DiscardValue() => ProtectedValue = default;

    #endregion

    #region Partial Implementation: Resolve

    public async Task<ISuccessResult> Commit()
    {
        var result = await SetImpl(ProtectedValue);
        OnCommitted(result, ProtectedValue);
        return result;
    }

    #endregion

    #region Abstract

    public abstract Task<ISetResult<TValue>> SetImpl(TValue value);

    #endregion

    #region Extensibility

    /// <summary>
    /// Raised when ReadCacheValue changes
    /// </summary>
    /// <param name="newValue"></param>
    /// <param name="oldValue"></param>
    protected virtual void OnValueChanged(TValue newValue, TValue oldValue) { }

    private void OnCommitted(object resolveResult, TValue protectedValue) => throw new NotImplementedException();

    #endregion
}

