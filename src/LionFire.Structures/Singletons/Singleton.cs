namespace LionFire.Structures;

public sealed class Singleton<T>
    where T : class
{
    public static T Instance => ManualSingleton<T>.GuaranteedInstance;
}
