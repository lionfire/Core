#nullable enable
using LionFire.Collections.Concurrent;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LionFire.Activities
{
    /// <summary>
    /// Tracks a collection of Activities, which are anything from short-lived async Tasks to long-running distributed Jobs
    /// </summary>
    public interface IActivitiesTracker
    {
        void Add(IActivity activity);
        IActivity Add(Task task, [System.Runtime.CompilerServices.CallerMemberName] string activityName = "", string? description = null);

        bool TryGetValue(Guid key, out IActivity? activity);
        void ClearHistory();
        bool ClearFinishedActivity(IActivity activity);
        ActivityStatus? Status { get; }

        IEnumerable<IActivity> ActivitiesOutstanding { get; }
        IEnumerable<IActivity> RecentlyFinished { get; }
        void ClearFinished(bool withErrors = false);
        

        event Action<ActivityBucket, IActivity>? ActivityBucketChanged;

        #region TODO

        ConcurrentList<IActivity>? History { get; }
        ConcurrentDictionary<Guid, IActivity>? Activities { get; }
        ConcurrentDictionary<Guid, IActivity>? InProgress { get; }

        #endregion

    }
}