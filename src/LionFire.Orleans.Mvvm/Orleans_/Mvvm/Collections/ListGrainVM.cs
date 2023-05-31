using LionFire.Mvvm;
using LionFire.Resolves;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Orleans_.Mvvm;

//public class OrleansListVM<TOutputItem, TValue> : AsyncCollectionVM<TOutputItem, TValue>
//    where TOutputItem : notnull
//{
//    #region Model

//    public IGrain Grain { get; }

//    #endregion

//    #region Lifecycle

//    public OrleansListVM(IGrain listGrain) : base(listGrain as ILazilyResolves<IEnumerable<KeyValuePair<TOutputItem, TValue>>> ?? throw new ArgumentException($"{nameof(listGrain)} must implement {nameof(ILazilyResolves<IEnumerable<KeyValuePair<TOutputItem, TValue>>>)}"))
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
//    #region Model

//    public IGrain Grain { get; }

//    #endregion

//    #region Lifecycle

//    public OrleansDictionaryVM(IGrain dictionaryGrain) 
//        //: base(dictionaryGrain as ILazilyResolves<IEnumerable<KeyValuePair<TKey, TValue>>> ?? throw new ArgumentException($"{nameof(dictionaryGrain)} must implement {nameof(ILazilyResolves<IEnumerable<KeyValuePair<TKey, TValue>>>)}"))
//    {
//        this.Grain = dictionaryGrain;
//    }

//    #endregion
//}