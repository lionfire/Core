namespace LionFire.Orleans_;


public interface IProvidesGrain<out TGrain>
{
    TGrain GetGrain(IGrainFactory grainFactory);
}
