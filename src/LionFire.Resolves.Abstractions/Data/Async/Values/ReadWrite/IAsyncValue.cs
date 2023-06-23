using LionFire.Data.Async.Sets;

namespace LionFire.Data.Async;

public interface IAsyncValue<T> 
    : IStatelessAsyncValue<T>
    , IGetsOrAsyncInstantiates<T>
{

}