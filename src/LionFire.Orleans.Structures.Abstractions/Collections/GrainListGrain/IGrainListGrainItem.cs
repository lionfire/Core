//using LionFire.Structures;

//namespace LionFire.Orleans_.Collections.ListGrain;

//public interface IGrainListGrainItem<out TValue> : IKeyed
//{
//    string Id { get; }
//}

namespace LionFire.Orleans_.Collections.GrainListGrain;

public interface IGrainListGrainItem<out TValue>
    : IProvidesGrain<TValue>
    , IHasGrainId
    where TValue : IGrain
{
    string Id { get; }
}

