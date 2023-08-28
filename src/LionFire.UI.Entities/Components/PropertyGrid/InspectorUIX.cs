namespace LionFire.Inspection.ViewModels;

public static class InspectorUIX
{
    public static bool EditorExistsForType(Type? type) // TODO - route this to IViewTypeProvider somehow
    {
        if (type == null) return false;

        if (type.IsGenericType)
        {
            var generic = type.GetGenericTypeDefinition();
            if (generic == typeof(Nullable<>))
            {
                return EditorExistsForType(type.GetGenericArguments()[0]);
            }
        }

        if (type == typeof(bool)
            || type == typeof(string)
            || type == typeof(int)
            || type == typeof(uint)
            || type == typeof(short)
            || type == typeof(ushort)
            || type == typeof(byte)
            || type == typeof(sbyte)
            || type == typeof(long)
            || type == typeof(ulong)
            || type == typeof(float)
            || type == typeof(double)
            || type == typeof(decimal)
            || type == typeof(DateTime)
            || type == typeof(DateTimeOffset)
            || type == typeof(TimeSpan)
            || type == typeof(char)
            || type == typeof(Guid)
            || type == typeof(Uri)
            )
            return true;
        return false;
    }

    public static bool DefaultCanExpand(Type type) // TODO: base this off of whether there are any child nodes
    {
        if (type == typeof(string)) return false;
        if (type == typeof(RuntimeTypeHandle)) return false;
        return true;
    }
}
