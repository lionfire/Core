using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Execution
{
    public interface IStartable
    {
        Task Start();
    }

    public static class IStartableExecutableExtensions
    {
        public static async Task Start(this IStartable startable, CancellationToken token)
        {
            // TODO: token
            await startable.Start();
        }
    }
}
