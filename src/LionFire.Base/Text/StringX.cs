
namespace LionFire;

public static class StringX
{
    public static string TrimEnd(this string str, string trimString)
    {
        if (str.EndsWith(trimString)) return str.Substring(0, str.Length - trimString.Length);
        return str;
    }
}
