#define SourceToTargetMap_off
using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace LionFire.Collections
{
    public interface IReadOnlyCollectionAdapter<T> : IReadOnlyCollection<T>, INotifyCollectionChanged
    {
        Type InstanceType { get; }
        Type SourceType { get; }
    }
}
