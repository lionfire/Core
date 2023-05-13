namespace LionFire.ExtensionMethods.Types;

public static class TypeNameExtensions
{
    public static string TrimLeadingIOnInterfaceType(this string type)
    {
        if (type.StartsWith("I") && type.Length > 1 && char.IsUpper(type[1])) { return type.Substring(1); }
        return type;
    }
}