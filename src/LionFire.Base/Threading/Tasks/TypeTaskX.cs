#nullable enable

namespace LionFire.ExtensionMethods;

public static class TypeTaskX
{
    public static bool IsTaskType(this Type? type)
    {
        if (type == null) return false;
        if (type.IsGenericType)
        {
            var genericType = type.GetGenericTypeDefinition();
            if (genericType.GetGenericArguments().Length != 1) { return false; }
            return genericType == typeof(Task<>)
#if NET6_0_OR_GREATER
                || genericType == typeof(ValueTask<>)
#endif
                ;
        }
        else
        {
            return type == typeof(Task)
#if NET6_0_OR_GREATER
                || type == typeof(ValueTask)
#endif
                ;
        }
    }
    public static Type? UnwrapTaskType(this Type? type)
    {
        if (type==null) return null;

        if (type.IsGenericType)
        {
            var genericType = type.GetGenericTypeDefinition();
            if (genericType.GetGenericArguments().Length != 1) { return null; }
            return (genericType == typeof(Task<>)
#if NET6_0_OR_GREATER
                || genericType == typeof(ValueTask<>)
#endif
                ) ? genericType.GetGenericArguments()[0] : null;
        }
        else
        {
            return (type == typeof(Task)
#if NET6_0_OR_GREATER
                || type == typeof(ValueTask)
#endif
                ) ? typeof(void) : null; ;
        }
    }
}
