using System.Threading.Tasks;
using Orleans;
using Orleans.Concurrency;

namespace LionFire.Orleans_.AspNetCore_;

[StatelessWorker(1)]
public class LocalHealthCheckGrain : Grain, ILocalHealthCheckGrain
{
    public Task PingAsync() => Task.CompletedTask;
}
