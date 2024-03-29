﻿using LionFire.Data.Async.Sets;

namespace LionFire.Data.Async;

public class FuncValue<TKey, TValue> : AsyncValue<TKey, TValue>
{
    public FuncValue(TKey key, Func<TKey, CancellationToken, Task<TValue>> getter, Func<TKey, TValue?, CancellationToken, Task> setter, ValueOptions? options = null) : base(key, options)
    {
        Getter = getter;
        Setter = setter;
    }

    #region Set

    public Func<TKey, TValue?, CancellationToken, Task> Setter { get; }

    public override async Task<ISetResult<TValue>> SetImpl(TValue? value, CancellationToken cancellationToken = default) 
    {
        try
        {
            await Setter(Key, value, cancellationToken).ConfigureAwait(false);
            return SetResult<TValue>.Success(value);
        }
        catch (Exception ex)
        {
            return SetResult<TValue>.FromException(ex, value);
        }
    }

    public Func<TKey, CancellationToken, Task<TValue>> Getter { get; }

    #endregion

    #region Get

    protected override async ITask<IGetResult<TValue>> GetImpl(CancellationToken cancellationToken = default)
    {
        if (Getter == null) return RetrieveResult<TValue>.NotInitialized;

        //return RetrieveResult<TValue>.NotInitialized; // TEMP FIXME
        try
        {
            var value = await Getter(this.Key, cancellationToken).ConfigureAwait(false);
            return RetrieveResult<TValue>.Success(value);
        }
        catch (Exception ex)
        {
            return RetrieveResult<TValue>.FromException(ex);
        }
    }
        
    #endregion

}
