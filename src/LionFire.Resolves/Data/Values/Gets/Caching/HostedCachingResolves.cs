// FUTURE
//using Microsoft.Extensions.Hosting;
//using MorseCode.ITask;
//using System.Threading;
//using System.Threading.Tasks;

//namespace LionFire.Data.Async.AsyncGetsWithEvents;

//public abstract class HostedCachingResolves<TValue> : LazilyResolves<TValue>, IHostedService
//{
//    public Task StartAsync(CancellationToken cancellationToken) => Get().AsTask();
//    public Task StopAsync(CancellationToken cancellationToken) { value = default; return Task.CompletedTask; }
//}
