#define SourceToTargetMap_off
using System.Collections.Generic;

namespace LionFire.Collections
{
    // Adapted from http://stackoverflow.com/a/15831128/208304


    public interface ICollectionAdapter<T> : ICollection<T>, IReadOnlyCollectionAdapter<T>
    {
    }
}
