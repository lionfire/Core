using System.Threading.Tasks;
using Orleans;

namespace LionFire.Orleans_.AspNetCore_
{
    public interface ILocalHealthCheckGrain : IGrainWithGuidKey
    {
        Task PingAsync();
    }
}
