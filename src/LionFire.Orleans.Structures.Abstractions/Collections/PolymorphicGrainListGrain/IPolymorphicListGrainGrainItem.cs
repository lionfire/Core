namespace LionFire.Orleans_.Collections.PolymorphicGrainListGrain;
#if UNUSED
public record GrainListItem<TValue, TMetadata>(string Id, Type Type)
    where TValue : IGrain
{
    /// <summary>
    /// Optional metadata for an item owned by the collection
    /// </summary>
    public TMetadata? Metadata { get; set; }

    public IGrainFactory GrainFactory { set => this.grain = (TValue?)value?.GetGrain(Type, Id); }

    public TValue? Grain => grain;
    //public TValue Grain { get => grain ??= (TValue)GrainFactory.GetGrain(Type, Id); set => this.grain = grain; }
    private TValue? grain;
}
#endif




