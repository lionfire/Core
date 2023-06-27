namespace LionFire.Data.Sync;

public interface IGetterSync<in TInput, out TOutput>
{
    IGetResult<TOutput> Get(TInput key);
}