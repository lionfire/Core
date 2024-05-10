using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Execution;

public interface IStartable
{
    Task StartAsync(CancellationToken cancellationToken = default);
}


//public static class IStartableExecutableExtensions
//{
//    public static async Task Start(this IStartable startable, CancellationToken token)
//    {
//        // TODO: token
//        await startable.Start(token).ConfigureAwait(false);
//    }
//}
