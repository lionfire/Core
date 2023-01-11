using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace LionFire.Types.Scanning;

public class TypeScannerOptions
{
    public bool EnableAssemblyScan { get; set; } = true;

    public List<string> DllPrefixBlacklist = new List<string>
    {
        "Microsoft.",
        "System.",
    };

    public List<string> DllPrefixWhitelist { get; set; } = new();

    /// <summary>
    /// Make sure the developer didn't forget to set up the Whitelist
    /// </summary>
    public bool RequireDefaultDllPrefixWhitelist { get; set; } = true;

    public Assembly[] AssemblyWhitelist { get; set; }

    /// <summary>
    /// For the typical case, throw out the scan results after startup
    /// </summary>
    public TimeSpan ScanCacheDuration { get; set; } = TimeSpan.FromSeconds(30);

    public void Validate()
    {
        if (RequireDefaultDllPrefixWhitelist && (DllPrefixWhitelist == null || DllPrefixWhitelist.Count == 0))
        {
            throw new ArgumentException($"{nameof(RequireDefaultDllPrefixWhitelist)} is true but {nameof(DllPrefixWhitelist)} is null or empty");
        }
    }
}
