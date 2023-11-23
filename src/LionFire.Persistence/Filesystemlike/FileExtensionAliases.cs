using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Filesystemlike;

public class FileExtensionAliases
{
    /// <summary>
    /// Key: alias  (example: pkg)
    /// Value: actual  (example: zip)
    /// Omit leading periods.
    /// </summary>
    public Dictionary<string, string> Aliases { get; set; } = new();

}

public static class FileExtensionAliasesX
{
    /// <summary>
    /// (Omit the leading period.)
    /// </summary>
    /// <param name="aliases"></param>
    /// <param name="actual"></param>
    /// <param name="alias"></param>
    /// <returns></returns>
    public static FileExtensionAliases Alias(this FileExtensionAliases aliases, string actual, string alias)
    {
        aliases.Aliases.Add(alias, actual);
        return aliases;
    }
}