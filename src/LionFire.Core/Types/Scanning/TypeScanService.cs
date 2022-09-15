//using Microsoft.Extensions.Hosting;
//using System.Threading;

//namespace LionFire.Types.Scanning;

//public class TypeScanService : IHostedService, IDisposable
//{
//    public Dictionary<string, TypeScanJob> Jobs { get; private set; } = new();

//    public void Dispose()
//    {
//        Jobs = null;
//    }

//    public Task<IEnumerable<Type>> ScanForTypes<TInterface>(TypeScanOptions options)
//    {
//        return Task.Run(async () =>
//        {
//            var job = new TypeScanJob(options);
//            return await job.Run();
//        });
//    }

//    public Task StartAsync(CancellationToken cancellationToken)
//    {
//        throw new NotImplementedException();
//    }

//    public Task StopAsync(CancellationToken cancellationToken)
//    {
//        Dispose();
//        return Task.CompletedTask;
//    }
//}
