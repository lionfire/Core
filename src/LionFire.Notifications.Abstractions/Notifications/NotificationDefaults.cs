using LionFire.Structures;
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



        //public static SoundProfile DefaultSoundProfile
        //{
        //    get { return new SoundProfile { Sounds = DefaultSounds }; }
        //}

        public static Dictionary<string, string> DefaultSounds
        {
            get
            {
                var sounds = new Dictionary<string, string>();
                sounds.Add("Message.", "bell_elevator_bell.mp3");
                return sounds;
            }
        }

        public static Dictionary<string,NotificationProfile> DefaultProfiles
        {
            get
            {
                var profiles = new Dictionary<string,NotificationProfile>();

                profiles.Add(new NotificationProfile("G3")
                {
                    Importance = Importance.Info,
// SoundProfile = "Alarm.Info",
                    Sound = "Bell/bell_elevator_bell.mp3",
//                    DefaultTitle = "Info",
                });


                return profiles;
            }
        }

        public static NotificationConfiguration DefaultConfiguration { get; set; } = new NotificationConfiguration
        {
            Profiles = DefaultProfiles,
            SoundPaths = new string[] {
                @"C:\st\Projects\Valor\Assets\Sound\02 ZapSplat", // STUB TEMP
            },
 //           Sounds = DefaultSounds,
        };
    }
}
