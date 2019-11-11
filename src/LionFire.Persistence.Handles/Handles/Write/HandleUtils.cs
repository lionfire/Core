#nullable enable
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        #region MutatePersistenceStateAndNotify

        internal static async Task<PersistenceEvent<TValue>?> MutatePersistenceStateAndNotify<TValue>(this INotifyingHandleInternal<TValue> h, Func<Task> action)
        {
            IPersists p = h;
            lock (h.PersistenceLock)
            {
                var oldValue = h.ProtectedValue;
                var oldHasValue = h.HasValue;
                var oldFlags = p.Flags;

                await action();

                if (
                    oldFlags != p.Flags
                    || oldHasValue != h.HasValue
                    || !EqualityComparer<TValue>.Default.Equals(oldValue, h.ProtectedValue)
                   )
                {
                    var ev = new PersistenceEvent<TValue>
                    {
                        New = new PersistenceSnapshot<TValue>(p.Flags, h.ProtectedValue, h.HasValue),
                        Old = new PersistenceSnapshot<TValue>(oldFlags, oldValue, oldHasValue),
                        Sender = h,
                    };

                    h.RaisePersistenceEvent(ev);
                    return ev;
                }
                else
                {
                    return default;
                }
            }
        }

        #endregion

        internal static void OnUserChangedValue_ReadWrite<TValue>(this IHandleInternal<TValue> h, TValue newValue)
        {
            IHandleInternal<TValue> hi = h;
            IPersists p = h;
            var newFlags = p.Flags;

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
            hi.Flags = newFlags;
        }

        internal static void OnUserChangedValue_Write<TValue>(this IHandleInternal<TValue> h, TValue newValue)
        {
            IPersists p = h;
            IHandleInternal<TValue> hi = h;

            var newFlags = p.Flags;

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
            hi.Flags = newFlags;
        }
    }
}
