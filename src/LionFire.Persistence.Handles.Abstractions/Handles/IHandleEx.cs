using LionFire.Structures;

namespace LionFire.Persistence
{
    public interface IHandle : IPersisted { }
    //public interface IHandle<T> : IPersisted<T> { }

    public interface IHandleEx : IHandle, IKeyed<string> { }
    //public interface IHandleEx<T> : IHandleEx, IHandle<T> { }
    
}
