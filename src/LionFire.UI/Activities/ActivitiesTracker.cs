#nullable enable
using LionFire.Collections.Concurrent;
using LionFire.Dependencies;
using LionFire.ExtensionMethods.Poco.Resolvables;
using LionFire.Structures;
using MediatR;
using Microsoft.Extensions.Logging;
using MorseCode.ITask;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace LionFire.Activities
{

    /// <summary>
    /// Tracks a collection of Activities, which are anything from short-lived async Tasks to long-running distributed Jobs
    /// </summary>
    public class ActivitiesTracker : INotifyPropertyChanged, IHandler<IActivity>, IActivitiesTracker
    {
        #region Dependencies

        public ILogger<ActivitiesTracker> Logger { get; }

        #endregion

        #region Parameters

        public ActivitiesTrackerOptions? Options { get; set; }

        #region Derived

        public ActivitiesTrackerOptions EffectiveOptions => Options ?? ActivitiesTrackerOptions.Default;

        #endregion

        #endregion

        #region Construction

        public ActivitiesTracker(ILogger<ActivitiesTracker> logger, IServiceProvider serviceProvider)
        {
            Logger = logger;

        }

        #endregion

        #region State

        // FUTURE - most activities probably won't support pausing
        public bool Paused { get => false; protected set => throw new NotImplementedException(); }

        // TODO: Make readonly
        public ConcurrentDictionary<Guid, IActivity>? Activities { get; private set; }
        // TODO: Make readonly
        public ConcurrentDictionary<Guid, IActivity>? InProgress { get; private set; }

        #region Ackable

        /// <summary>
        /// A cache of recently finished non-foreground activities, as well as all foreground activities that haven't been manually cleared
        /// </summary>
        public IEnumerable<IActivity> AckRequired => ackRequired ?? Enumerable.Empty<IActivity>();
        private ConcurrentList<IActivity> ackRequired = new();

        public void AckAll(bool withErrors = false)
        {
            foreach (var activity in ackRequired.Where(a => a.IsCompleted).ToArray())
            {
                if (!withErrors && activity.Status?.HasError == true) { continue; }
                ackRequired?.Remove(activity);
            }
        }

        #endregion

        #region RecentlyFinished

        /// <summary>
        /// A cache of recently finished non-foreground activities, as well as all foreground activities that haven't been manually cleared
        /// </summary>
        public IEnumerable<IActivity> RecentlyFinished => recentlyFinished ?? Enumerable.Empty<IActivity>();
        private ConcurrentList<IActivity> recentlyFinished = new();

        public void ClearFinished(bool withErrors = false)
        {
            foreach (var activity in recentlyFinished.Where(a => a.IsCompleted).ToArray())
            {
                if (!withErrors && activity.Status?.HasError == true) { continue; }
                recentlyFinished?.Remove(activity);
            }
        }

        #endregion

        #region Finished Activities

        public bool ClearFinishedActivity(IActivity activity)
        {
            var result1 = recentlyFinished.Remove(activity);
            var result2 = ackRequired.Remove(activity);
            return result1 || result2;
        }

        #endregion

        // TODO: Make readonly
        public ConcurrentList<IActivity>? History { get => history; }
        private ConcurrentList<IActivity>? history;

        public void ClearHistory() => history?.Clear();

        #region Derived

        /// <summary>
        /// TODO - Not fully Implemented
        /// True if Paused is true and all activities (if any) are paused.
        /// False if Paused is false.
        /// Null if Paused is true and some activities are paused and some aren't (because they don't support it).
        /// </summary>
        public bool? EffectivePaused { get => Paused; }


        public ActivityStatus? Status
        {
            // OPTIMIZE: Cache this and only change when children or children statuses change
            get
            {
                if (InProgress?.Any() == true)
                {
                    if (InProgress.Count > 1)
                    {
                        var children = InProgress.Values.Select(a => a.Status ?? ActivityStatus.NullFallback).ToList();
                        return new ActivityStatus
                        {
                            Text = TextForChildren(InProgress.Values, false),
                            Children = children,
                            Progress = 0, // TODO: Aggregate
                        };
                    }
                    else
                    {
                        return InProgress.First().Value.Status ?? ActivityStatus.NullFallback;
                    }

                }
                else if (RecentlyFinished?.Any() == true)
                {
                    var children = RecentlyFinished.Select(a => a.Status ?? ActivityStatus.NullFallback).ToList();
                    return new ActivityStatus
                    {
                        Text = TextForChildren(RecentlyFinished, true),
                        Children = children,
                        Progress = 1,
                    };
                }
                else
                {
                    return ActivityStatus.Idle;
                }
            }
        }

        /// <summary>
        /// True if activities are in progress and not paused.
        /// Null if activities are in progress and all paused.
        /// False if no activities are in progress.
        /// </summary>
        public bool? IsDoingSomething
        {
            get
            {
                if (InProgress?.Any() == true)
                {
                    return EffectivePaused == true ? null : true;
                }
                else
                {
                    return false;
                }
            }
        }

        #endregion

        #endregion

        #region IHandler<IActivity>

        public void Handle(IActivity activity) { Add(activity); }

        #endregion

        public bool TryGetValue(Guid key, out IActivity? activity)
        {
            if (Activities != null)
            {
                return Activities.TryGetValue(key, out activity);
            }
            else
            {
                activity = default;
                return false;
            }
        }

        public IEnumerable<IActivity> ActivitiesOutstanding => Activities?.Values.Where(j => !j.IsCompleted) ?? Enumerable.Empty<IActivity>();

        public void Add(IActivity activity)
        {
            if (activity == null)
            {
                Logger.LogWarning($"Add got null activity");
                return;
            }

            bool hadExistingKey;
            var keyable = activity as IKeyable<Guid>;

            if (activity.Key == default)
            {
                hadExistingKey = false;
                if (keyable == null) { throw new ArgumentException("IActivity has unset Key but is not IKeyable"); }
                keyable.Key = Guid.NewGuid();
            }
            else
            {
                hadExistingKey = true;
            }

            Activities ??= new ConcurrentDictionary<Guid, IActivity>();

            while (!Activities.TryAdd(activity.Key, activity))
            {
                throw new ArgumentException("IActivity Key is already registered");
                if (hadExistingKey)
                {
                    throw new ArgumentException("IActivity Key is already registered");
                }
                else
                {
                    keyable.Key = Guid.NewGuid();
                }
            }

            Logger.LogInformation($"Activity: {activity}");

            if (activity?.Task == null)
            {
                OnTaskFinished(activity);
            }
            else
            {
                activity.Task.ContinueWith(t => OnTaskFinished(activity));
            }

            ActivityBucketChanged?.Invoke(ActivityBucket.InProgress, activity);
        }

        public IActivity Add(Task task, [System.Runtime.CompilerServices.CallerMemberName] string activityName = "", string? description = null)
        {
            // REVIEW - REFACTOR with Add(IActivity)
            TaskActivity activity;
            Guid guid;
            do
            {
                guid = Guid.NewGuid();
                activity = new TaskActivity()
                {
                    Key = guid,
                    Name = activityName,
                    Description = description,
                    Task = task,
                };

                Activities ??= new ConcurrentDictionary<Guid, IActivity>();

            } while (false == Activities.TryAdd(guid, activity));

            task.ContinueWith(t => OnTaskFinished(activity));

            ActivityBucketChanged?.Invoke(ActivityBucket.InProgress, activity);

            return activity;
        }

        #region (Public) Events

        public event Action<ActivityBucket, IActivity>? ActivityBucketChanged;

        #endregion

        #region Event Handlers

        private void OnTaskFinished(IActivity activity)
        {
            Logger.LogInformation($"Activity finished: {activity}");

            Activities?.TryRemove(activity.Key, out _);
            recentlyFinished ??= new();
            recentlyFinished.Add(activity);
            ActivityBucketChanged?.Invoke(ActivityBucket.Finished, activity);

            history ??= new();
            history.Add(activity);

            if ((activity.Options?.Foreground) != true)
            {
                Task.Delay(EffectiveOptions.FinishedStatusMessageDuration).ContinueWith(t =>
                {
                    recentlyFinished.Remove(activity);
                    ActivityBucketChanged?.Invoke(ActivityBucket.RemovedFromRecentlyFinished, activity);
                });
            }
            // else, stays in recentlyFinished until user clears it
        }

        #endregion

        #region Text Helpers

        public string TextForChildren(IEnumerable<IActivity> activities, bool finished)
        {
            var statuses = activities.Select(a => a.Status ?? ActivityStatus.NullFallback).ToList();

            var text = (finished ? "Finished: " : "In progress: ") + activities.Select(s => s.Name ?? s.Key.ToString()).Aggregate((x, y) => $"{x}, {y}");
            if (text.Length <= EffectiveOptions.MaxTextLength) { return text; }

            text = $"{statuses.Count()} tasks {(finished ? "finished" : "in progress")}";
            if (text.Length <= EffectiveOptions.MaxTextLength) { return text; }

            text = $"{statuses.Count()} {(finished ? "done" : "tasks")}";
            if (text.Length <= EffectiveOptions.MaxTextLength) { return text; }

            text = finished ? "Done" : statuses.Count().ToString();
            if (text.Length <= EffectiveOptions.MaxTextLength) { return text; }

            return "";
        }

        #endregion

        #region Misc

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #endregion

        #endregion
    }

}