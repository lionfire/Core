using LionFire.Referencing;

namespace LionFire.Persistence.Handles
{
    public interface IHandleBase : IReferencable, IPersists
    {
    }

    //public interface IHandle<T> : IPersisted<T> { }

}
