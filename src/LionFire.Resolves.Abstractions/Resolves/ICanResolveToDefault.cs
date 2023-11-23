#if UNUSED
namespace LionFire.Resolvables;

public interface ICanResolveToDefault
{
    /// <summary>
    /// Successfully resolved but to the default value (such as null, or 0)
    /// </summary>
    bool ResolvedToDefault { get; }
}

#endif