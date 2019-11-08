using System;

namespace LionFire.Persistence.Handles
{
    //public enum WriteFlexibilityPolicy
    //{
    //    None = 0,
    //    CreateToUpdate = 1 << 0,
    //    UpdateToCreate = 1 << 1,
    //    //UpsertToUpdate = 1 << 2,
    //    //UpsertToCreate = 1 << 3,
    //    IgnoreEtag = 1 << 4,

    //    Default = CreateToUpdate | UpdateToCreate,
    //    All = CreateToUpdate | UpdateToCreate 
    //        //| UpsertToUpdate | UpsertToCreate // Are these useful?
    //        | Etag,
    //}

    internal static class HandleUtils
    {
        internal static void OnUserChangedValue_ReadWrite<TValue>(IHandleInternal<TValue> h, TValue newValue)
        {
            MutatePersistenceState(h, () =>
            {
                var newFlags = h.Flags;

                // REVIEW these flags.  TODO: Add a strict mode for Create/Update/Upsert, and also etags for strict update of etag

                if (newValue == default)
                {
                    newFlags |= PersistenceFlags.OutgoingDeletePending;
                    newFlags &= ~(PersistenceFlags.OutgoingUpsertPending | PersistenceFlags.OutgoingCreatePending | PersistenceFlags.OutgoingUpdatePending);
                }
                else
                {
                    if (newFlags.HasFlag(PersistenceFlags.NotFound))
                    {
                        newFlags |= PersistenceFlags.OutgoingCreatePending;
                        newFlags &= ~(PersistenceFlags.OutgoingDeletePending | PersistenceFlags.OutgoingUpdatePending | PersistenceFlags.OutgoingUpsertPending);
                    }
                    else if (newFlags.HasFlag(PersistenceFlags.Found))
                    {
                        newFlags |= PersistenceFlags.OutgoingUpdatePending;
                        newFlags &= ~(PersistenceFlags.OutgoingDeletePending | PersistenceFlags.OutgoingCreatePending | PersistenceFlags.OutgoingUpsertPending);
                    }
                    else
                    {
                        newFlags |= PersistenceFlags.OutgoingUpsertPending;
                        newFlags &= ~(PersistenceFlags.OutgoingDeletePending | PersistenceFlags.OutgoingCreatePending | PersistenceFlags.OutgoingUpdatePending);
                    }
                }

                h.ProtectedValue = newValue;
            });
        }

        internal static void OnUserChangedValue_Write<TValue>(IHandleInternal<TValue> h, TValue newValue)
        {
            MutatePersistenceState(h, () =>
            {
                var newFlags = h.Flags;

                if (newValue == default)
                {
                    newFlags |= PersistenceFlags.OutgoingDeletePending;
                    newFlags &= ~(PersistenceFlags.OutgoingUpsertPending | PersistenceFlags.OutgoingCreatePending | PersistenceFlags.OutgoingUpdatePending);
                }
                else
                {
                    newFlags |= PersistenceFlags.OutgoingUpsertPending;
                    newFlags &= ~(PersistenceFlags.OutgoingDeletePending | PersistenceFlags.OutgoingCreatePending | PersistenceFlags.OutgoingUpdatePending);
                }

                h.ProtectedValue = newValue;
            });
        }
        internal static PersistenceEvent<TValue> MutatePersistenceState<TValue>(this IHandleInternal<TValue> h, Action action)
        {
            lock (h.persistenceLock)
            {
                var oldValue = h.ProtectedValue;
                var oldHasValue = h.HasValue;
                var oldFlags = h.Flags;

                action();

                return new PersistenceEvent<TValue>
                {
                    New = new PersistenceSnapshot<TValue>(h.Flags, h.ProtectedValue, h.HasValue),
                    Old = new PersistenceSnapshot<TValue>(oldFlags, oldValue, oldHasValue),
                    Sender = h,
                };
            }
        }
    }
}
