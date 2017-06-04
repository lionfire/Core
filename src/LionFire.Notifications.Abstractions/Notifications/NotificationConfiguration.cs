using System.Collections.Generic;

namespace LionFire.Notifications
{
    public class NotificationConfiguration
    {
        public Dictionary<string, NotificationProfile> Profiles { get; set; }

        public IEnumerable<string> SoundPaths { get; set; }
    }
}
