#nullable enable
using System.Text.RegularExpressions;

namespace LionFire;

public static class StringX
{
    public static string TrimEnd(this string str, string trimString)
    {
        if (str.EndsWith(trimString)) return str.Substring(0, str.Length - trimString.Length);
        return str;
    }

    // OPTIMIZE - if moving this dll out of netstandard2 to net7+
    //[GeneratedRegex("([a-z])([A-Z])", "en-US")]
    //private static partial Regex AbcOrDefGeneratedRegex();

    // Replace spaces with hyphens and convert to lowercase
    public static string? ToKebabCase(this string? input) => input == null ? null 
        : Regex.Replace(input, @"([a-z])([A-Z])", "$1-$2")
                     .Replace(" ", "-")
                     .Replace("_", "-")
                     .Replace("--", "-") // Handle multiple consecutive hyphens
                     .ToLower();

    //Regex.Replace(input, @"\s+", "-").ToLower(); // REVIEW - verify regex
    // Alternate: @"([a-z])([A-Z])", "$1-$2").ToLowerInvariant();

}
