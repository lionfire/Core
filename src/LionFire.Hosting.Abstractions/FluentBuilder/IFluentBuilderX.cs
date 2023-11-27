using System.Runtime.CompilerServices;

namespace LionFire.Hosting;

public static class IFluentBuilderX // MOVE
{
    public static T InitStatics<T>(this T b, Action a)
        where T : IFluentBuilder
    {
        a();
        return b;
    }

    public static T Configure<T>(this T @this, Action<T> configure)
        where T : IFluentBuilder
    {
        configure(@this);
        return @this;
    }

}
