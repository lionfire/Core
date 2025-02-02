using System;
using System.Collections.Concurrent;

namespace LionFire.Schemas.Runtime;

public static class ResolvedTypeAliasX
{
    private static ConcurrentDictionary<TypeAlias, ResolvedTypeAlias> cache = new ConcurrentDictionary<TypeAlias, ResolvedTypeAlias>();

    public static ResolvedTypeAlias Resolve(TypeAlias typeAlias)
    {
        return cache.GetOrAdd(typeAlias, alias =>
        {
            var type = Type.GetType(alias.Type ?? throw new ArgumentNullException(nameof(alias)) );
            return new ResolvedTypeAlias(alias, type);
        });
    }
}


