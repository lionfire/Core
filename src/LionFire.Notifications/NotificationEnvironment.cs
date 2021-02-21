using System;
using System.IO;

namespace LionFire.Notifications.Wpf
{
    public class NotificationEnvironment
    {
        public const string NotificationsSubPath = "Notifications";

        public const string DesktopNotificationQueue = "DesktopAlerts";

        public static string DesktopAlertQueueDir
        {
            get
            {
                throw new NotImplementedException();
                //return Path.GetFullPath(Path.Combine(
                //    LionFireEnvironment.Directories.CompanyProgramData,
                //    NotificationsSubPath,
                //    "localhost",
                //    DesktopNotificationQueue
                //    ));
            }
        }
    }
}
