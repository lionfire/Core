using LionFire.Orleans_.Collections.GrainListGrain;

namespace LionFire.Orleans_.Collections.ListGrain;

public sealed record GrainListGrainItem<TValue>(string Id) :
        IEquatable<IGrainListGrainItem<TValue>>,
        IEquatable<GrainListGrainItem<TValue>>,
        IGrainListGrainItem<TValue>
    where TValue : IGrainWithStringKey
{
    public string Key => Id;

    string IHasGrainId.GrainPrimaryKey => Id;

    #region Construction

    public static GrainListGrainItem<TValue> FromKey(string key) => new GrainListGrainItem<TValue>(key);

    #endregion

    public TValue GetGrain(IGrainFactory grainFactory) => grainFactory.GetGrain<TValue>( Id);

    #region Misc

    public override string ToString() => $"{Key}";

    public bool Equals(IGrainListGrainItem<TValue>? obj)
    {
        var other = obj as IGrainListGrainItem<TValue>;
        if (other == null) return false;

        return other.Id == Id;
    }
    public bool Equals(GrainListGrainItem<TValue>? obj)
    {
        var other = obj as GrainListGrainItem<TValue>;
        if (other == null) return false;

        return other.Id == Id;
    }

    #endregion
}
