using LionFire.Data.Async.Gets;
using LionFire.Persistence;
using LionFire.Results;
using MorseCode.ITask;
using System;
using System.Threading.Tasks;

namespace LionFire.Data.Async.Sets;


/// <summary>
/// Only requires one method to be implemented: CommitImpl.
/// 
/// If setting to null or default is a legitimate operation, use DefaultableValue&lt;TValue&gt;
/// </summary>
[Obsolete("Use AsyncSets")] // OLD
public abstract class Commits<TKey, TValue> : DisposableKeyed<TKey>, IDiscardableValue
    where TKey : class
    where TValue : class
{
    #region Construction

    protected Commits() { }
    protected Commits(TKey input) : base(input) { }

    #endregion

    #region Value

    /// <summary>
    /// For nullable values, use TValue of DefaultableValue&lt;TValue&gt;
    /// </summary>
    public bool HasValue => ProtectedValue != default;

    public TValue Value => ProtectedValue;

    protected TValue ProtectedValue
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
    private TValue protectedValue;

    #endregion

    #region Discard

    public virtual void DiscardValue() => ProtectedValue = default;

    #endregion

    #region Partial Implementation: Resolve

    public async Task<ISuccessResult> Commit()
    {
        var result = await CommitImpl(ProtectedValue);
        OnCommitted(result, ProtectedValue);
        return result;
    }

    #endregion

    #region Abstract

    public abstract Task<IPersistenceResult> CommitImpl(TValue value);

    #endregion

    #region Extensibility

    /// <summary>
    /// Raised when ProtectedValue changes
    /// </summary>
    /// <param name="newValue"></param>
    /// <param name="oldValue"></param>
    protected virtual void OnValueChanged(TValue newValue, TValue oldValue) { }

    private void OnCommitted(object resolveResult, TValue protectedValue) => throw new NotImplementedException();

    #endregion
}

