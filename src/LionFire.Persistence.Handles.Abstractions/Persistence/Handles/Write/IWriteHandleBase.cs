using LionFire.Data.Gets;
using LionFire.Data.Sets;
using LionFire.Structures;

namespace LionFire.Persistence
{
    /// <summary>
    /// IWriteHandleBase
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IWriteHandleBase<T> 
        : IContravariantWriteHandleBase<T>
        , IDefaultableWrapper<T>
        , IStagesSet<T>
        , ISets<T>
    {
    }
}
