using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Execution
{
    public interface IStoppable
    {
        Task Stop(CancellationToken? cancellationToken = null);
    }
}
