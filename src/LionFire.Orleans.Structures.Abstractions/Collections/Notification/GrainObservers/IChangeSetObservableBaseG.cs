using DynamicData;
using Orleans.Runtime;

namespace LionFire.Orleans_.Collections;

///// <summary>
///// Implemented by a grain that is observable via a corresponding Subscribe method
///// </summary>
//public interface IChangeSetObservableBaseG
//    : IAddressable
//{
//    ValueTask UnsubscribeAsync(IAddressable addressable);
//}

//public interface IChangeSetObservableG<TValue>
//    : IChangeSetObservableBaseG
//    , IGrainObservableG<ChangeSet<TValue>>
//{
//}

//public interface IChangeSetObservableG<TValue, TKey>
//    : IChangeSetObservableBaseG
//    , IGrainObservableG<ChangeSet<TValue, TKey>>
//    where TKey : notnull
//{
//}