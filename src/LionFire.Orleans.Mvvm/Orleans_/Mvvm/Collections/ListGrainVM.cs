using LionFire.Mvvm;
using LionFire.Data.Async.Gets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Orleans_.Mvvm;

//public class OrleansListVM<TOutputItem, TValue> : AsyncCollectionVM<TOutputItem, TValue>
//    where TOutputItem : notnull
//{
//    #region Value

//    public IGrain Grain { get; }

//    #endregion

//    #region Lifecycle

//    public OrleansListVM(IGrain listGrain) : base(listGrain as ILazilyGets<IEnumerable<KeyValuePair<TOutputItem, TValue>>> ?? throw new ArgumentException($"{nameof(listGrain)} must implement {nameof(ILazilyGets<IEnumerable<KeyValuePair<TOutputItem, TValue>>>)}"))
//    {

//        this.Grain = listGrain;
//    }

//    #endregion

//    #region IObservableCreatesAsync

//    //public override IObservable<(Type, object[], Task<KeyValuePair<TOutputItem, TValue>>)> Creates => throw new NotImplementedException();

//    //public override Task<KeyValuePair<TOutputItem, TValue>> Create(Type type, params object[] constructorParameters)
//    //{
//    //    throw new NotImplementedException();
//    //}

//    #endregion
//}

//public class OrleansDictionaryVM<TKey, TValue> : AsyncDictionaryVM<TKey, TValue>
//    where TKey : notnull
//{
//    #region Value

//    public IGrain Grain { get; }

//    #endregion

//    #region Lifecycle

//    public OrleansDictionaryVM(IGrain dictionaryGrain) 
//        //: base(dictionaryGrain as ILazilyGets<IEnumerable<KeyValuePair<TKey, TValue>>> ?? throw new ArgumentException($"{nameof(dictionaryGrain)} must implement {nameof(ILazilyGets<IEnumerable<KeyValuePair<TKey, TValue>>>)}"))
//    {
//        this.Grain = dictionaryGrain;
//    }

//    #endregion
//}