namespace LionFire.Synchronization
{

    public interface ISyncConnectionSettings
    {

        ConflictResolutionMode ConflictResolutionMode { get; set; }

        double DesiredNotificationResponseTime { get; set; }

        /// <summary>
        /// i.e.  High precedence to spam polling, Normal precedence accepts normal defaults for the current sync mode
        /// </summary>
        double DesiredNotificationResponsePrecedence { get; set; }
    }

}
