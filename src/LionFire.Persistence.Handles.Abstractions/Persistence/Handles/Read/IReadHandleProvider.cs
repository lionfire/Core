using LionFire.Referencing;

namespace LionFire.Persistence.Handles
{
    public interface IReadHandleProvider
    {
        IReadHandleBase<T> GetReadHandle<T>(IReference reference, T handleObject = default);
    }

    //[AutoRegister]
    public interface IReadHandleProvider<TReference>
        where TReference : IReference
    {
        IReadHandleBase<T> GetReadHandle<T>(TReference reference, T handleObject = default);
    }

}
