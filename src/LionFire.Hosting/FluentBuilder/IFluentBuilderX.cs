namespace LionFire.Hosting;

public static class IFluentBuilderX // MOVE
{
    public static T InitStatics<T>(this T b, Action a)
        where T : IFluentBuilder
    {
        a();
        return b;
    }
}
