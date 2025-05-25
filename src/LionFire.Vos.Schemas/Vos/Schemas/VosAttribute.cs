using System.Reflection;

namespace LionFire.Vos.Schemas;

public enum VosFlags
{
    Unspecified = 0,

    PreferDirectory = 1 << 0,

}

public class VosAttribute : Attribute
{
    public VosFlags Flags { get; }

    public VosAttribute(VosFlags flags)
    {
        Flags = flags;
    }

}

public static class VosSchema
{

    public static VosFlags Flags<T>() 
        => typeof(T).GetCustomAttribute<VosAttribute>()?.Flags 
        ?? VosFlags.Unspecified;

}
