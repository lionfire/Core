namespace LionFire.Data;

public class FuncAsyncValue<TKey, TValue> : Value<TKey, TValue>
{
    public FuncAsyncValue(TKey key, Func<TKey, CancellationToken, Task<TValue>> getter, Func<TKey, TValue?, CancellationToken, Task> setter, AsyncValueOptions? options = null) : base(key, options)
    {
        Getter = getter;
        Setter = setter;
    }

    public override async Task<ITransferResult> SetImpl(TKey key, TValue? value, CancellationToken cancellationToken = default)
    {
        try
        {
            await Setter(this.Key, value, cancellationToken).ConfigureAwait(false);
            return TransferResult.Success;
        }
        catch (Exception ex)
        {
            return TransferResult.FromException(ex);
        }
    }

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

    public Func<TKey, CancellationToken, Task<TValue>> Getter { get; }


    public Func<TKey, TValue?, CancellationToken, Task> Setter { get; }

}
