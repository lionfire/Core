namespace LionFire.Synchronization
{
    public interface ISyncConnectionMode
    {
        string Name { get; }
        string Description { get; }

        bool ProvidesChangeNotifications { get; }

        double AverageDelayMilliseconds { get; }
        double TypicalMinDelayMilliseconds { get; }
        double TypicalMaxDelayMilliseconds { get; }
        double TypicalDelayPercentWithinMinMax { get; }

        //Local Disk I/O
        //  AvgDelay
        //    5ms
        //  Name
        //    Local Disk I/O
        //Cloud sync
        //  Typical delay
        //    5s
        //Distributed sync
        //  Typical delay
        //    10s-24h
        //Weak sync
        //  ReceivesNotifications
        //    false
    }

}
