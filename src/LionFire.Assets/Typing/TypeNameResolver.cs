#nullable enable

using System;

namespace LionFire.Typing
{
    public static class TypeNameResolver
    {
        // TODO: TreatAsAttribute
        // TODO: Use a bidirectional Type Alias resolution service.

        public static string ToName(this Type type) => type.FullName;
    }
}
