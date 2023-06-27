
using System.Reactive.Subjects;

namespace LionFire.Data.Async;

// OLD - folded into AsyncValue<,>
//public abstract class AsyncProperty<TObject, TValue> : AsyncValue<TObject, TValue>
//{
//    #region Lifecycle

//    public AsyncProperty(TObject target, AsyncValueOptions? options = null) : base(target, options)
//    {        
//    }

//    #endregion    
//}


//public class AsyncProperty_Old<TObject, TValue> : AsyncValue<TObject, TValue>
//{
//    #region ILazilyGets

//    #region Value

//    //public TValue? Value
//    //{
//    //    //[Blocking(Alternative = nameof(GetIfNeeded))]
//    //    get
//    //    {
//    //        if (!HasValue)
//    //        {
//    //            if (Options.GetOnDemand)
//    //            {
//    //                if (Options.BlockToGet)
//    //                {
//    //                    Debugger.NotifyOfCrossThreadDependency();
//    //                    return Get().Result;
//    //                }
//    //                else
//    //                {
//    //                    Get().FireAndForget();
//    //                }
//    //            }
//    //            else if (Options.ThrowOnGetValueIfHasValueIsFalse) { DoThrowOnGetValueIfNotLoaded(); }
//    //        }
//    //        return cachedValue;
//    //    }
//    //    set
//    //    {
//    //        //cachedValue = value;
//    //        this.RaiseAndSetIfChanged(ref cachedValue, value);
//    //    }
//    //}
//    //private TValue? cachedValue;

//    #endregion
        
//    //public async ITask<ILazyGetResult<TValue>> GetIfNeeded() 
//    //{
//    //    if (HasValue) return new LazyResolveNoopResult<TValue>(HasValue, Value);
//    //    var value = await this.Get().ConfigureAwait(false);
//    //    return new LazyResolveResult<TValue>(true, value);
//    //}

//    //public ILazyGetResult<TValue> QueryValue() => new LazyResolveNoopResult<TValue>(HasValue, Value);

//    //public ITask<IGetResult<TValue>> Resolve()
//    //{
//    //    lock (getLock)
//    //    {
//    //        var task = gets.Value;
//    //        if (!task.AsTask().IsCompleted) { return task; }

//    //        task = Task.Run<IGetResult<TValue>>(async () =>
//    //        {
//    //            var task = Getter(Target, default);
//    //            //gets.OnNext(task);
//    //            var value = await task.ConfigureAwait(false);
//    //            Value = value;
//    //            hasValue.OnNext(true);
//    //            return new ResolveResultSuccess<TValue>(value);
//    //        }).AsITask();
//    //        gets.OnNext(task);
//    //        return task;
//    //    }
//    //}

//    #endregion

//    //private void DoThrowOnGetValueIfNotLoaded() => throw new Exception("Value has not been gotten yet.  Invoke Get first or disable Options.ThrowOnGetValueIfNotLoaded");
       

//}

