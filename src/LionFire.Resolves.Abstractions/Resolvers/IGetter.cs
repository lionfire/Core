
namespace LionFire.Data;

public interface IGetter<in TInput, out TOutput> 
{
    ITask<IGetResult<TOutput>> Resolve(TInput key, CancellationToken cancellationToken = default);
}

