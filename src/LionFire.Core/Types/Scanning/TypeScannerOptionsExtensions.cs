using System.Linq;
using System.Reflection;

namespace LionFire.Types.Scanning;

public static class TypeScannerOptionsExtensions
{
    //public static List<string> DefaultDllPrefixWhitelist => new List<string>
    //{
    //    "FireLynx.", // DEPRECATED
    //};
    
    public static bool PassesPrefixFilter(this TypeScannerOptions o, Assembly a)
    {
        if (o.DllPrefixWhitelist != null && o.DllPrefixWhitelist.Count > 0)
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

    public static bool PassesFilter(this TypeScannerOptions o, Assembly a)
    {
        return PassesPrefixFilter(o, a);
    }

    public static IEnumerable<Assembly> GetAssemblies(this TypeScannerOptions o)
    {
        if (o.AssemblyWhitelist != null) { return o.AssemblyWhitelist; }
        if (!o.EnableAssemblyScan) { return Enumerable.Empty<Assembly>(); }
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        return assemblies.Where(a => o.PassesPrefixFilter(a));
    }

}
