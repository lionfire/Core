using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Notifications
{
    public enum NotificationStateFlags
    {
        None = 0,
        Submitted = 1 << 1,
        Delivered = 1 << 2,
        Seen = 1 << 3,

        Snoozed = 1 << 4,

        Acked = 1 << 5,

        /// <summary>
        /// Implies acked
        /// </summary>
        Dismissed = 1 << 6,


        Expired = 1 << 7,

    }

    public interface IExpiringNotification
    {
        bool IsExpired { get; }

    }
}
