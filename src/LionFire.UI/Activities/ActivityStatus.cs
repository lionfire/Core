#nullable enable
using System;
using System.Collections.Generic;

namespace LionFire.Activities
{
   
    public partial class ActivityStatus
    {
        #region Static

        public static ActivityStatus NullFallback => new ActivityStatus()
        {
            Text = "(null)",
        };

        public static ActivityStatus Idle => new ActivityStatus()
        {
            Text = "Ready",
        };

        #endregion


        public string? Text { get; set; }
        public string? Icon { get; set; }

        public List<ActivityStatus>? Children { get; set; }

        public double? Progress { get; set; }
        public bool IsFinished => Progress != null && Progress.Value >= 1;

        public DateTimeOffset Start { get; set; }

        public TimeSpan Elapsed => DateTimeOffset.UtcNow - Start;

        public TimeSpan? ExpectedDuration { get; set; }

        #region REVIEW: better way to sort out error/warning results?
        
        public bool HasError { get; set; }
        public bool HasWarning { get; set; }

        #endregion
    }
}