using LionFire.Referencing;

namespace LionFire.Persistence.Handles
{
    public interface IReadHandleProvider
    {
        IReadHandle<T> GetReadHandle<T>(IReference reference);
    }

    //[AutoRegister]
    public interface IReadHandleProvider<TReference>
        where TReference : IReference
    {
        IReadHandle<T> GetReadHandle<T>(TReference reference);
    }

}
