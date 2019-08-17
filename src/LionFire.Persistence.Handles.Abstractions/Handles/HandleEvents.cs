using System;

namespace LionFire
{
    [Flags]
    public enum HandleEvents
    {
        None = 0,

        StateChanged = 1 << 0,

        ObjectReferenceChangedByHandle = 1 << 1,
        ObjectReferenceChangedBySource = 1 << 2,

        ObjectPropertiesChangedViaObject = 1 << 3,
        ObjectPropertiesChangedFromSource = 1 << 4,

        ObjectCommittedToSource = 1 << 5,
        ObjectRetrievedFromSource = 1 << 6,

        CommittedObjectCreate = 1 << 7,
        CommittedObjectDelete = 1 << 8,

        ObjectDeletedInSource = 1 << 9,

        ObjectForgotten = 1 << 10,


        HandleDisposed = 1 << 12,
    }
}
