
namespace LionFire.Data;

public interface IGetter<in TInput, out TOutput> 
{
    ITask<IGetResult<TOutput>> Get(TInput key, CancellationToken cancellationToken = default);
}

