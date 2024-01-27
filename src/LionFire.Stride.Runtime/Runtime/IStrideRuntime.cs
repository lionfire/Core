using LionFire.Stride_.Core;
using Microsoft.Extensions.Hosting;
using Stride.Core.Serialization.Contents;
using System.Threading;

namespace LionFire.Stride_.Runtime;

public interface IStrideRuntime : ITypedServiceProvider, IHostedService
{
    ContentManager Content { get; }
    Task Load();
    InheritingServiceRegistry StrideServices { get; }

    CancellationToken IsStarted { get;}
}

