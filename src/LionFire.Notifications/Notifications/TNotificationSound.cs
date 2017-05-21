using System.Collections.Generic;

namespace LionFire.Notifications
{
    public class TNotificationSound
    {
        public bool Repeating { get; set; }

        /// <summary>
        /// Delay in milliseconds between end of sound and start of playing it again
        /// </summary>
        public int RepeatDelayMilliseconds { get; set; }

        public int DelayBeforeFirstSound { get; set; }

        /// <summary>
        /// Subpath in asset/vos dir?
        /// </summary>
        public string SoundPath { get; set; }
        //public List<string> SoundPaths

    }
}
