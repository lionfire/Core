using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Reflection;

namespace LionFire.Types.Scanning;

public class TypeScanOptions
{
    public bool EnableAssemblyScan { get; set; } = true;

    public List<string> DllPrefixBlacklist = new List<string>
    {
        "Microsoft.",
        "System.",
    };

    public List<string>? DllPrefixWhitelist { get; set; }

    public Assembly[] AssemblyWhitelist { get; set; }

    /// <summary>
    /// For the typical case, throw out the scan results after startup
    /// </summary>
    public TimeSpan ScanCacheDuration { get; set; } = TimeSpan.FromSeconds(30);

}

public static class TypeScanOptionsExtensions
{
    public static List<string> DefaultDllPrefixWhitelist => new List<string>
    {
        "LionFire.",
        "FireLynx.",
    };

    public static IServiceCollection ConfigureDefaults(this IServiceCollection services)
        => services.Configure<TypeScanOptions>(o =>
        {
            o.DllPrefixWhitelist ??= new();
            o.DllPrefixWhitelist.AddRange(DefaultDllPrefixWhitelist);
        });

    public static bool PassesPrefixFilter(this TypeScanOptions o, Assembly a)
    {
        if (o.DllPrefixWhitelist != null)
        {
            foreach (var item in o.DllPrefixWhitelist)
            {
                if (a?.FullName?.StartsWith(item) == true) return true;
            }
            return false;
        }

        if (o.DllPrefixBlacklist != null)
        {
            foreach (var item in o.DllPrefixBlacklist)
            {
                if (a?.FullName?.StartsWith(item) == true) return false;
            }
        }

        return true;
    }

    public static IEnumerable<Assembly> GetAssemblies(this TypeScanOptions o)
    {
        if (o.AssemblyWhitelist != null) { return o.AssemblyWhitelist; }
        if (!o.EnableAssemblyScan) { return Enumerable.Empty<Assembly>(); }
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        return assemblies.Where(a => o.PassesPrefixFilter(a));
    }
}
