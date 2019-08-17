using LionFire.Referencing;

namespace LionFire.Persistence.Handles // RENAME LionFire.Referencing.Handles
{
    public interface IReadHandleProvider
    {
        RH<T> GetReadHandle<T>(IReference reference, T handleObject = default);
    }

    //[AutoRegister]
    public interface IReadHandleProvider<TReference>
        where TReference : IReference
    {
        RH<T> GetReadHandle<T>(TReference reference, T handleObject = default);
    }
    
}
