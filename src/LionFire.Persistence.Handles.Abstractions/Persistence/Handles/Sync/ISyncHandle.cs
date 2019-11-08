using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Persistence.Handles.Sync
{
    // TODO
    public interface ISyncHandleBase<T> : IReadWriteHandleBase<T>
    {
    }

    public enum SyncHandleConflictResolutionPolicy
    {
    }

    public enum SyncConflictResolutionMethod
    {
        Unspecified = 0,
    }

    public interface ISyncHandle<T> : IReadWriteHandle<T>
    {
        SyncHandleConflictResolutionPolicy ConflictResolutionPolicy { get; set; }

        void ResolveSyncConflict(T obj);

    }
}
