using LionFire.Data.Sets;

namespace LionFire.Data;

public interface IAsyncValue<T> 
    : IStatelessAsyncValue<T>
    , IGetsOrAsyncInstantiates<T>
{

}