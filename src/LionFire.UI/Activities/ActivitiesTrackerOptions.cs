using System;

namespace LionFire.Activities
{
    public class ActivitiesTrackerOptions
    {
        public static ActivitiesTrackerOptions Default { get; } = new ActivitiesTrackerOptions();

        public TimeSpan FinishedStatusMessageDuration { get; set; } = TimeSpan.FromMinutes(1);

        public TimeSpan PreserveHistoryDuration { get; set; } = TimeSpan.FromMinutes(3);

        public int MaxTextLength { get; set; } = 80;
    }

}