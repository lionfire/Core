using System;

namespace LionFire.Schemas.Runtime;

public class ResolvedTypeAlias
{
    public ResolvedTypeAlias(TypeAlias typeAlias, Type? type)
    {
        TypeAlias = typeAlias;
        Type = type;
    }

    public TypeAlias TypeAlias { get; }
    public System.Type? Type { get; }
}


