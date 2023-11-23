using LionFire.Results;
using System.Threading.Tasks;

namespace LionFire.Data.Async.Gets;

//public abstract class ResolvesBaseCovariant<TKey, TValue> : AsyncGetsWithEvents<TKey, TValue, object>, ILazilyGetsCovariant<TValue>
//{
//}


///// <summary>
///// Typical implementation of Resolvable, implementing ILazyRetrievable
///// </summary>
///// <typeparam name="TInput"></typeparam>
///// <typeparam name="TOutput"></typeparam>
//public abstract class ResolvesBaseCovariant<TKey, TValue> : DisposableKeyed<TKey>, IResolvesEx, ILazilyGetsCovariant<TValue>, ILazilyResolves<TValue>
//{
//    public static implicit operator ResolvesBaseCovariant<TKey, TValue>(TKey input) => new ResolvesBaseCovariant<TKey, TValue>(input);

//    public ResolvesBaseCovariant() { }
//    public ResolvesBaseCovariant(TKey input) : base(input) { }


//    //#region State

//    //public PersistenceState State
//    //{
//    //    get => state;
//    //    set
//    //    {
//    //        if (state == value) return;
//    //        state = value;
//    //        // TODO: Implement IHasPersistenceStateEvents
//    //    }
//    //}
//    //private PersistenceState state;

//    //#endregion

//    #region Output

//    public TValue Value
//    {
//        get => Get().Result.Object;
//        protected set => @object = value;
//    }
//    private TValue @object;

//    #region RetrievedNull

//    public bool RetrievedNull
//    {
//        get => retrievedNull;
//        protected set => retrievedNull = value;
//    }
//    private bool retrievedNull;

//    #endregion

//    #endregion

//    #region Input

//    public bool HasValue => !ReferenceEquals(@object, default) || RetrievedNull;

//    public async Task<(bool HasObject, TValue Object)> Get()
//    {
//        if (state.HasFlag(PersistenceState.UpToDate))
//        {
//            return (true, @object);
//        }

//        await Get();

//        return (HasValue, @object);
//    }
//    async Task<(bool HasObject, object Object)> ILazilyGetsCovariant<TValue>.Get()
//    {
//        var result = await this.Get();
//        return (result.HasObject, result.Object);
//    }

//    public void DiscardValue()
//    {
//        retrievedNull = false;
//        @object = default;
//    }

//    public Task<IResolveResult> Get()
//    {
//        this.Value = Key.Get<TKey, TValue>();
//        return SuccessResult.Success.ToResult<IResolveResult>();
//    }

//    public Task<bool> Exists(bool forceCheck = false) => throw new System.NotImplementedException();

//    #endregion
//}
