using System;

namespace LionFire.Hosting;

public static class FluentExtensions
{
    public static T If<T>(this T builder, bool condition, Action<T> conditionTrue)
    {
        if (condition)
        {
            conditionTrue(builder);
        }        
        return builder;
    }
    public static T IfElse<T>(this T builder, bool condition, Action<T>? @if, Action<T>? @else)
    {
        if (condition)
        {
            @if?.Invoke(builder);
        }
        else
        {
            @else?.Invoke(builder);
        }
        return builder;
    }
}
