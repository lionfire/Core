
namespace LionFire.Data.Gets;

public interface ILazilyGetsCovariant<out T> : IStatelessGets<T>, IReadWrapper<T> {

//    Task<ILazyGetResult<object /* T */>> GetValue();
}
