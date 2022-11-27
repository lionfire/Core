namespace LionFire.Orleans_.Collections.PolymorphicGrainListGrain;

public record PolymorphicGrainListGrainItem<TValue>(string Id, Type Type) :
        IEquatable<IPolymorphicGrainListGrainItem<TValue>>,
        IEquatable<PolymorphicGrainListGrainItem<TValue>>,
        IPolymorphicGrainListGrainItem<TValue>
    where TValue : IGrain
{
    //#region REVIEW - Inject GrainFactory to resolve IGrain (only store Grain)

    ////public void ResolveGrain(IGrainFactory grainFactory) { this.grain = (TValue?)grainFactory?.GetGrain(Type, Id);  }
    //public IGrainFactory GrainFactory { set => this.grain = (TValue?)value?.GetGrain(Type, Id); }
    //IGrainFactory IDependsOn<IGrainFactory>.Dependency { set => GrainFactory = value; }
    //public TValue? Grain => grain;
    //private TValue? grain;

    //#endregion

    //public string Key => $"{Id}({Type.FullName})";
    public string Key => Id;

    string IHasGrainId.GrainPrimaryKey => Id;

    #region Construction

    public static PolymorphicGrainListGrainItem<TValue> FromKey(string key)
    {
        var split = key.Split("(");
        if (split.Length != 2) { throw new ArgumentException(); }
        var type = split[1].TrimEnd(')');
        return new PolymorphicGrainListGrainItem<TValue>(split[0], Type.GetType(type) ?? throw new ArgumentException($"Failed to resolve type: {type}"));
    }

    #endregion

    public TValue GetGrain(IGrainFactory grainFactory)
        => (TValue)grainFactory.GetGrain(Type, Id);

    #region Misc

    public override string ToString() => $"{Key} ({Type.Name})";

    public bool Equals(IPolymorphicGrainListGrainItem<TValue>? obj)
    {
        IPolymorphicGrainListGrainItem<TValue>? other = obj as IPolymorphicGrainListGrainItem<TValue>;
        if (other == null) return false;

        return other.Id == Id && other.Type == Type;
    }

    #endregion
}
