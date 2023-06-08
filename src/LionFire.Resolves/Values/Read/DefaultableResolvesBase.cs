using System.Threading.Tasks;

namespace LionFire.Data.Async.Gets
{
    // OLD - Defaultable classes unneeded. Just wrap with DefaultableValue<T>

    //public abstract class ResolvesWithoutEvents<TKey, TValue> : DefaultableResolvesBaseBase<TKey, TValue>
    //{
    //    public override bool HasValue => hasValue;
    //    protected bool hasValue { get; set; }

    //    protected override TValue value { get; set; }

    //    public override void DiscardValue()
    //    {
    //        lock (_lock)
    //        {
    //            value = default;
    //            hasValue = false;
    //        }
    //    }

    //    public override async Task<IResolveResult<TValue>> Resolve()
    //    {
    //        var resolveResult = await ResolveImpl();
    //        lock (_lock)
    //        {
    //            hasValue = resolveResult.HasValue;
    //            value = resolveResult.Value;
    //        }
    //        ValueChanged?.Invoke(resolveResult.Value, resolveResult.HasValue, oldValue, oldHasValue);
    //        return resolveResult;
    //    }

    //    public abstract Task<IResolveResult<TValue>> ResolveImpl();
    //}

    //public class DefaultableResolves<TKey, TValue> : DefaultableResolvesBase<TKey, TValue>
    //{
    //    protected override TValue value { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    //    public Func<TKey, Task<IResolveResult<TValue>>> Resolver { get; set; }

    //    public override Task<IResolveResult<TValue>> ResolveImpl() => return Resolver(this.Key);
    //}

    //public abstract class NotifyingDefaultableResolvesBase<TKey, TValue> : DisposableKeyed<TKey>, ILazilyResolves<TValue>, INotifiesSenderValueChanged<DefaultableValue<TValue>>
    //{
    //    public event EventHandler<ValueChanged<DefaultableValue<TValue>>> ValueChanged;
    //}

    //public abstract class DefaultableResolvesBase<TKey, TValue> : DisposableKeyed<TKey>, ILazilyResolves<TValue>
    //{
    //    #region Construction

    //    protected DefaultableResolvesBaseBase() { }
    //    protected DefaultableResolvesBaseBase(TKey input) : base(input) { }

    //    #endregion

    //    protected DefaultableValue<TValue> defaultableValue { get; set; }

    //    public abstract bool HasValue { get; }
    //    public virtual TValue Value => GetValue().Result.Value;
    //    protected abstract TValue value { get; set; }

    //    public virtual async Task<ILazyResolveResult<TValue>> GetValue()
    //    {
    //        var currentValue = defaultableValue;
    //        if (currentValue.HasValue) return new LazyResolveResultNoop<TValue>(value);

    //        var resolveResult = await Resolve();
    //        return new LazyResolveResult<TValue>(resolveResult.HasValue, resolveResult.Value);
    //    }

    //    public override void DiscardValue()
    //    {
    //        var oldValue = defaultableValue;
    //        defaultableValue = DefaultableValue<TValue>.Default;
    //        ValueChanged?.Invoke(this, (DefaultableValue<TValue>.Default, oldValue));
    //    }

    //    public override async Task<IResolveResult<TValue>> Resolve()
    //    {
    //        var oldValue = defaultableValue;

    //        var resolveResult = await ResolveImpl();
    //        lock (_lock)
    //        {
    //            defaultableValue = resolveResult.HasValue;
    //            hasValue = resolveResult.HasValue;
    //            value = resolveResult.Value;
    //        }
    //        ValueChanged?.Invoke(resolveResult.Value, (resolveResult.HasValue, oldValue));
    //        return resolveResult;
    //    }

    //    public abstract Task<IResolveResult<TValue>> ResolveImpl();
    //}

}

