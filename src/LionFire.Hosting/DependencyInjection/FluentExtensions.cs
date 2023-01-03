using System;

namespace LionFire.Hosting;

public static class FluentExtensions
{
    public static T If<T>(this T builder, bool condition, Action<T> action)
    {
        if (condition)
        {
            action(builder);
        }
        return builder;
    }
}
