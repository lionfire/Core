#if OLD
using LionFire.Orleans_.Collections.GrainListGrain;
using LionFire.Orleans_.Collections.ListGrain;

namespace LionFire.Orleans_.Collections.PolymorphicGrainListG;

public interface IPolymorphicGrainListGrainItem<out TValue> : IGrainListGrainItem<TValue>
        
    where TValue : IGrain
{
    Type Type { get; }
}


//public interface IPolymorphicGrainListGrainItem<out TValue>
//    : IPolymorphicGrainListGrainItem<TValue>

//    where TValue : IGrain
//{
//}
#endif