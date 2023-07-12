
namespace LionFire.Data.Gets;

public interface ILazilyGetsCovariant<out T> : IGets<T>, IReadWrapper<T> {

//    Task<ILazyGetResult<object /* T */>> GetValue();
}
