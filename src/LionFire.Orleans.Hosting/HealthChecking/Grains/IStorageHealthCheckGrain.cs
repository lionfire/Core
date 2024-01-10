using System.Threading.Tasks;
using Orleans;
using Orleans.Concurrency;

namespace LionFire.Orleans_.AspNetCore_;

public interface IStorageHealthCheckGrain : IGrainWithStringKey
{
    [ReadOnly]
    Task CheckAsync();
}
