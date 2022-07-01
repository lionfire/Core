using System.Threading.Tasks;
using Orleans;

namespace LionFire.Orleans_.AspNetCore_
{
    public interface IStorageHealthCheckGrain : IGrainWithGuidKey
    {
        Task CheckAsync();
    }
}
