using System.Text.RegularExpressions;

namespace LionFire.Deployment;

public record ReleaseChannel(string Id, string Name, int Precedence)
{
    public string KebabCaseId => ToKebabCase(Id);

    public static string ToKebabCase(string s) => Regex.Replace(s, @"([a-z])([A-Z])", "$1-$2").ToLowerInvariant();

    public override string ToString() => $"{Id} ({Name} - precedence: {Precedence})";
}


