namespace LionFire.Orleans_.Collections;

// REVIEW - 
/// <summary>
/// 
/// </summary>
/// <typeparam name="TMetadata"></typeparam>
/// <param name="Id">Grain primary string key</param>
/// <param name="Type">Grain interface type</param>
public record PolymorphicListEntry<TMetadata>(string Id, Type Type)
{
    /// <summary>
    /// Optional metadata for an item owned by the collection
    /// </summary>
    public TMetadata? Metadata { get; set; }
}

public record PolymorphicListEntry(string Id, Type Type);
