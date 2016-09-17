using LionFire.Applications.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Applications
{
    public interface IAppTask : IAppComponent
    {        
        void Start(CancellationToken? cancellationToken = null);

        bool WaitForCompletion { get; }

        Task Task { get; }
        
    }
}
