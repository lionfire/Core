namespace LionFire.Orleans_;

public static class IHasGrainIdExtensions
{
    public static TGrain GetGrain<TGrain>(this IHasGrainId has, IGrainFactory grainFactory)
        where TGrain : IGrainWithStringKey
        => grainFactory.GetGrain<TGrain>(has.GrainPrimaryKey);
}


