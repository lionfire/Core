using LionFire.Referencing;

namespace LionFire.Persistence.Handles
{
    public interface IReadHandleProvider
    {
        IReadHandleBase<T> GetReadHandle<T>(IReference reference);
    }

    //[AutoRegister]
    public interface IReadHandleProvider<TReference>
        where TReference : IReference
    {
        IReadHandleBase<T> GetReadHandle<T>(TReference reference);
    }

}
