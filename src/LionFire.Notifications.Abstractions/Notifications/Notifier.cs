using System;

namespace LionFire.Notifications
{
    public interface INotifier
    {
    }

    /// <summary>
    /// TODO: think about how to create a generic "SeriousAlerts" SDK.  Integrate with the PriceAlert.
    /// </summary>
    public class Notifier : INotifier
    {
        #region Construction

        public Notifier() { }
        public Notifier(Importance importance, Urgency urgency) {
            this.Importance = importance;
            this.Urgency = urgency;
        }

        #endregion

        public virtual string Key { get; set; }
        public virtual string CalculateKey() { return null; }
        public NotificationFlags Flags { get; set; }

        public Importance Importance { get; set; }
        public Urgency Urgency { get; set; }

        /// <summary>
        /// Description for the notifier, such as how it is related to some other event or why it exists
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// A custom  message to display.  May be ignored by subtypes.
        /// REVIEW - don't have this? Move to derived classes?
        /// </summary>
        public string Message { get; set; }
        
        public string Profile { get; set; }

        /// <summary>
        /// If null, keep raising this notificiation when relevant 
        /// </summary>
        public int? NotificationsRemaining { get; set; } // TODO

        /// <summary>
        /// If the condition is true, and stays true or becomes true again, create another notification this amount of time afterwards.  By default, prior notifications should be superceded.
        /// </summary>
        public TimeSpan TimeBetweenNotifications { get; set; } // TODO

        public override string ToString()
        {
            var msg = Message != null ? $" '{Message}'" : "";
            return $"!{Importance.ToCode()}-{Urgency.ToCode()} {Key} {msg}";
        }


    }
}
