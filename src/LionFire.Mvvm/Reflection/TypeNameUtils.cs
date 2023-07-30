namespace LionFire.Reflection;

public static class TypeNameUtils // MOVE?
{
    /// <summary>
    /// Display text for the Type of the member
    /// </summary>
    public static string DisplayTypeName(this Type type)
    {
        var genericTypeDefinition = type.IsGenericType ? type.GetGenericTypeDefinition() : null;
        if (genericTypeDefinition == null) return type.Name;

        if (genericTypeDefinition == typeof(IObservable<>))
        {
            return type.GetGenericArguments()[0].Name;
        }
        else if (genericTypeDefinition == typeof(IAsyncEnumerable<>))
        {
            return type.GetGenericArguments()[0].Name;
        }
        else if (genericTypeDefinition.Name == "SettablePropertyAsync`2" || genericTypeDefinition.Name == "PropertyAsync`2")
        {
            return type.GetGenericArguments()[1].Name;
        }
        return type.Name;
    }
}
