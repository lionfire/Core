using LionFire.Data.Async.Sets;

namespace LionFire.Data.Async;

public interface IAsyncValueEx<T> 
    : IAsyncValue<T>
    , IGetsOrAsyncInstantiates<T>
{

}