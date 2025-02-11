
using System.Text.RegularExpressions;

namespace LionFire;

public static class StringX
{
    public static string TrimEnd(this string str, string trimString)
    {
        if (str.EndsWith(trimString)) return str.Substring(0, str.Length - trimString.Length);
        return str;
    }

    public static string ToKebabCase(this string input)
    {
        // Replace spaces with hyphens and convert to lowercase
        return Regex.Replace(input, @"\s+", "-").ToLower();
    }
}
