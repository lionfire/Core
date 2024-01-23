using LionFire.Stride_.Core;
using Stride.Core.Serialization.Contents;

namespace LionFire.Stride_.Runtime;

public interface IStrideRuntime : ITypedServiceProvider
{
    ContentManager Content { get; }
    Task Load();
    InheritingServiceRegistry StrideServices { get; }

}

