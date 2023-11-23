#nullable enable
using LionFire.Data.Async.Sets;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LionFire.Persistence.Handles;

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

internal static class ReadCacheValueUtils
{
    /// <summary>
    /// Wraps a mutation of ProtectedValue and raises an event if the value changed.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="stagesSet"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    /// <remarks>
    /// TODO: Change ProtectedValue to ReadCacheValue
    /// TODO: Change First Parameter to IGets, and test for persistence flag tracking (IPersistsInternal)
    /// </remarks>
    internal static PersistenceEvent<TValue>? StageSetAndNotify<TValue>(this IStagesSetWithPersistenceFlags<TValue> stagesSet, Action action)
    {
        // REVIEW this method and the interfaces involved.

        //lock (stagesSet.PersistenceLock) // TODO: THREADSAFETY
        {
            var oldValue = stagesSet.StagedValue;
            var oldHasValue = stagesSet.HasStagedValue;
            var oldFlags = stagesSet.Flags;

            action();

            if (
                oldFlags != stagesSet.Flags
                || oldHasValue != stagesSet.HasStagedValue
                || !EqualityComparer<TValue>.Default.Equals(oldValue, stagesSet.StagedValue)
               )
            {
                var ev = new PersistenceEvent<TValue>
                {
                    New = new PersistenceSnapshot<TValue>(stagesSet.Flags, stagesSet.StagedValue, stagesSet.HasStagedValue),
                    Old = new PersistenceSnapshot<TValue>(oldFlags, oldValue, oldHasValue),
                    Sender = stagesSet,
                };

                ((INotifyPersistsInternal<TValue>)stagesSet).RaisePersistenceEvent(ev); // CAST TODO avoid this stealth cast
                return ev;
            }
            else
            {
                return default;
            }
        }
    }
}

internal static class StagedValueUtils
{

    internal static void StageValue_ReadWrite<TValue>(this IStagesSet<TValue> h, TValue newValue)
    {
        PersistenceFlags newFlags = default;
        IPersistsInternal p = h as IPersistsInternal;

        if (p != null)
        {
            newFlags = p.Flags;

            // REVIEW these flags.  TODO: Add a strict mode for Create/Update/Upsert, and also etags for strict update of etag

            if (EqualityComparer<TValue>.Default.Equals(newValue, default(TValue)))
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
        }

        h.StagedValue = newValue;

        if (p != null)
        {
            p.Flags = newFlags;
        }
    }

    #region DEDUPE

    [Obsolete("See StageValue_Write")]
    internal static void StageValue_Write_Old<TValue>(this IHandleInternal<TValue> h, TValue newValue)
    {
        //if (h is IStagesSet<TValue> ss) { StageValue_Write(s, newValue); } // TODO: Reconcile

        PersistenceFlags newFlags = default;
        if (h is IPersistsInternal p)
        {
            newFlags = p.Flags;

            if (EqualityComparer<TValue>.Default.Equals(newValue, default))
            {
                newFlags |= PersistenceFlags.OutgoingDeletePending;
                newFlags &= ~(PersistenceFlags.OutgoingUpsertPending | PersistenceFlags.OutgoingCreatePending | PersistenceFlags.OutgoingUpdatePending);
            }
            else
            {
                newFlags |= PersistenceFlags.OutgoingUpsertPending;
                newFlags &= ~(PersistenceFlags.OutgoingDeletePending | PersistenceFlags.OutgoingCreatePending | PersistenceFlags.OutgoingUpdatePending);
            }
        }

        ((IStagesSet<TValue>)h).StagedValue = newValue; // STEALTH CAST

        if (h is IPersistsInternal pi)
        {
            pi.Flags = newFlags;
        }
    }

    internal static void StageValue_Write<TValue>(this IStagesSet<TValue> s, TValue newValue)
    {
        PersistenceFlags newFlags = default;

        IPersistsInternal p = s as IPersistsInternal;
        if (p != null)
        {
            newFlags = p.Flags;

            if (EqualityComparer<TValue>.Default.Equals(newValue, default))
            {
                newFlags |= PersistenceFlags.OutgoingDeletePending;
                newFlags &= ~(PersistenceFlags.OutgoingUpsertPending | PersistenceFlags.OutgoingCreatePending | PersistenceFlags.OutgoingUpdatePending);
            }
            else
            {
                newFlags |= PersistenceFlags.OutgoingUpsertPending;
                newFlags &= ~(PersistenceFlags.OutgoingDeletePending | PersistenceFlags.OutgoingCreatePending | PersistenceFlags.OutgoingUpdatePending);
            }
        }

        s.StagedValue = newValue;
        if (p != null)
        {
            p.Flags = newFlags;
        }
    }
    #endregion

}

internal static class StagesSetUtils
{

}