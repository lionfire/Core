using LionFire.Data.Async.Collections;
using Newtonsoft.Json.Linq;

namespace LionFire.Orleans_.Collections;

//public interface IGrainListG<TItemG>
//    : IListG<TItemG>
//    , ICreatesG<TItemG>
//    , IGrainObservableG<ChangeSet<TItemG, GrainId>>
//    where TItemG : IGrain
//{
//}

#if ENH
/// <summary>
/// (ENH - NOT IMPLEMENTED)
/// This is for Dictionaries with IAddressable values where you supply your own key (e.g. a string that is relevant to your domain)
/// </summary>
public interface IAddressableDictionaryG<TKey, TItemG>
{
}

/// <summary>
/// (ENH - NOT IMPLEMENTED)
/// This is for Dictionaries with IGrain values where you supply your own key (e.g. a string that is relevant to your domain)
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TItemG"></typeparam>
public interface IGrainDictionaryG<TKey, TItemG>
    : IDictionaryG<TKey, TItemG>
    , ICreatesG<TItemG>
    where TKey : notnull
    where TItemG : IGrain
{
}

/// <summary>
/// (ENH - NOT IMPLEMENTED)
/// This is for Dictionaries with arbitrary value types where you supply your own key (e.g. a string that is relevant to your domain)
/// </summary>
/// <typeparam name="TItemG"></typeparam>
public interface IDictionaryG<TKey, TItem>
    : IKeyedListG<TKey, KeyValuePair<TKey, TItem>>
    , IChangeSetObservableG<TItem, TKey>
    //ICreatingAsyncDictionary<string, PolymorphicGrainListGrainItem<TItemG>> 
    //, IEnumerable<TItemG>
    //where TItemG : IGrain
    where TKey : notnull
{
    
}
#endif