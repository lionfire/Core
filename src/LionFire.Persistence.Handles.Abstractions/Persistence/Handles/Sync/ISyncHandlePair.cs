using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Persistence.Handles.Sync
{
    public interface ISyncHandlePairBase<T> : IReadWriteHandlePairBase<T>
    {
    }
    public interface ISyncHandlePair<T> : IReadWriteHandlePair<T>
    {
    }
}
