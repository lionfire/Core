using System.Collections.Generic;

namespace LionFire.Notifications
{

    public static class NotificationDefaults
    {

        public static IEnumerable<string> DefaultSearchPaths
        {
            get
            {
                yield return @"C:\st\Projects\Valor\Assets\Sound\02 ZapSplat";
            }
        }

        public static SoundProfile DefaultSoundProfile
        {
            get { return new SoundProfile { Sounds = DefaultSounds }; }
        }

        public static Dictionary<string, string> DefaultSounds
        {
            get
            {
                var sounds = new Dictionary<string, string>();

                sounds.Add("Alarm.Info", "bell_elevator_bell.mp3");
                return sounds;
            }
        }

        public static List<NotificationProfile> DefaultProfiles
        {
            get
            {
                var profiles = new List<NotificationProfile>();

                profiles.Add(new NotificationProfile("InfoAlarm")
                {
                    Importance = NotificationSeverity.Info,
                    SoundProfile = "Alarm.Info",
                    DefaultTitle = "Info",
                });

                return profiles;
            }
        }
    }
}
