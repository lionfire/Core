using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Notifications
{
    public enum NotificationSeverity
    {
        Unspecified,
        Trace,
        Debug,
        Info,
        Caution,

        Message,
        Warning,
        Error,
        Critical,
        Supercritical
    }
}
