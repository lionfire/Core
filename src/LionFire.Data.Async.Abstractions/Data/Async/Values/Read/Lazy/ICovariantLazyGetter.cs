
namespace LionFire.Data.Async.Gets;

public interface ICovariantLazyGetter<out T> : IStatelessGetter<T>, IReadWrapper<T> {

//    Task<IGetResult<object /* T */>> GetValue();
}
