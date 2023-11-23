using System.Diagnostics.CodeAnalysis;

namespace LionFire.Orleans_;

public class GrainEqualityComparer : IEqualityComparer<IAddressable>
{
    public static GrainEqualityComparer Instance { get; } = new GrainEqualityComparer();
    public bool Equals(IAddressable? x, IAddressable? y) => x?.GetGrainId().Equals(y?.GetGrainId()) == true;

    public int GetHashCode([DisallowNull] IAddressable obj) => obj.GetGrainId().GetHashCode();
}