using LionFire.Referencing;
using LionFire.Assets;
using LionFire.Persistence;

namespace LionFire.Vos.VosApp
{

    /// <summary>
    /// Static accessors, for convenience
    /// </summary>
    public static class VosAppSettings
    {
        public static class App<T> 
        {
            public static IReadWriteHandle<T> H
                => AssetReference<T>.ForChannel("$AppSettings").GetReadWriteHandle<T>();
            public static IReadHandle<T> R
                 => AssetReference<T>.ForChannel("$AppSettings").GetReadHandle<T>();
        }
        public static class User<T>
        {
            public static IReadWriteHandle<T> H
                => AssetReference<T>.ForChannel("$UserSettings").GetReadWriteHandle<T>();
            public static IReadHandle<T> R
                 => AssetReference<T>.ForChannel("$UserSettings").GetReadHandle<T>();
        }
        public static class UserLocal<T>
        {
            public static IReadWriteHandle<T> H
                => AssetReference<T>.ForChannel("$UserLocalSettings").GetReadWriteHandle<T>();
            public static IReadHandle<T> R
                 => AssetReference<T>.ForChannel("$UserLocalSettings").GetReadHandle<T>();
        }
    }
}
