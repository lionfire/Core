using LionFire.Mvvm;
using LionFire.Data.Async.Gets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Orleans_.Mvvm;

//public class OrleansListVM<TOutputItem, TInfo> : AsyncCollectionVM<TOutputItem, TInfo>
//    where TOutputItem : notnull
//{
//    #region Value

//    public IGrain Grain { get; }

//    #endregion

//    #region Lifecycle

//    public OrleansListVM(IGrain listGrain) : base(listGrain as ILazilyGets<IEnumerable<KeyValuePair<TOutputItem, TInfo>>> ?? throw new ArgumentException($"{nameof(listGrain)} must implement {nameof(ILazilyGets<IEnumerable<KeyValuePair<TOutputItem, TInfo>>>)}"))
//    {

//        this.Grain = listGrain;
//    }

//    #endregion

//    #region IObservableCreatesAsync

//    //public override IObservable<(Type, object[], Task<KeyValuePair<TOutputItem, TInfo>>)> Creates => throw new NotImplementedException();

//    //public override Task<KeyValuePair<TOutputItem, TInfo>> Create(Type type, params object[] constructorParameters)
//    //{
//    //    throw new NotImplementedException();
//    //}

//    #endregion
//}

//public class OrleansDictionaryVM<TKey, TInfo> : AsyncDictionaryVM<TKey, TInfo>
//    where TKey : notnull
//{
//    #region Value

//    public IGrain Grain { get; }

//    #endregion

//    #region Lifecycle

//    public OrleansDictionaryVM(IGrain dictionaryGrain) 
//        //: base(dictionaryGrain as ILazilyGets<IEnumerable<KeyValuePair<TKey, TInfo>>> ?? throw new ArgumentException($"{nameof(dictionaryGrain)} must implement {nameof(ILazilyGets<IEnumerable<KeyValuePair<TKey, TInfo>>>)}"))
//    {
//        this.Grain = dictionaryGrain;
//    }

//    #endregion
//}