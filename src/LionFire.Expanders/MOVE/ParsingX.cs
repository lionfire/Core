namespace LionFire.ExtensionMethods.Parsing;

public static class ParsingX
{
    public static bool TryStripPrefix(this string prefix, ref string str)
    {
        if (str.StartsWith(prefix)) { str = str.Substring(prefix.Length); return true; }
        else { return false; }
    }
}
