#nullable enable
namespace LionFire.Hosting;

public static class HostingX
{
    public static T Do<T>(this T me, Action<T> action) { action(me); return me; } // MOVE

}