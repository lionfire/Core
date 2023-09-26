namespace LionFire.ExtensionMethods.Orleans_;

public static class OrleansX
{
    public static bool IsOrleansProxy(this Type? type)
        => type != null && type.FullName?.StartsWith("OrleansCodeGen.") == true && type.Name.StartsWith("Proxy_");
}
