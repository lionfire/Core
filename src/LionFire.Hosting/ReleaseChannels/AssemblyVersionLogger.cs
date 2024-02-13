using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace LionFire.Hosting;

public class AssemblyVersionLoggerOptions
{
    public Assembly? RootAssembly { get; set; }
    public Type? ProgramType { get; set; }

    public bool SkipSignedAssemblies { get; set; } = true;

    /// <summary>
    /// In this case the prefix is the first segment of the assembly name, up to the first dot.
    /// </summary>
    public HashSet<string> PrefixWhitelist { get; set; } = new();

    public int? MaxDepth { get; set; }
}

public class AssemblyVersionLogger(ILogger<AssemblyVersionLogger> logger, IOptionsMonitor<AssemblyVersionLoggerOptions> options) : IHostedService
{
    private void DoLog()
    {
        var sb = new StringBuilder();

        var rootAsssembly = options.CurrentValue.RootAssembly ?? Assembly.GetEntryAssembly();

        if (rootAsssembly == null)
        {
            logger.LogWarning("RootAssembly is null");
            return;
        }


        SortedList<DateTime, Assembly> list = new();

        Recurse(rootAsssembly, list);

        if (list.Count > 0)
        {
            sb.AppendLine($"Assemblies:");
            foreach (var kvp in list.Reverse())
            {
                LogAssembly(sb, kvp.Value);
            }
            logger.LogInformation(sb.ToString());
        }
        else
        {
            logger.LogDebug("No assemblies to log");
        }
    }
    void Recurse(Assembly assembly, SortedList<DateTime, Assembly> list, int depth = 1)
    {
        if (options.CurrentValue.MaxDepth.HasValue && depth > options.CurrentValue.MaxDepth.Value) return;
        if (assembly.FullName == null) return;
        if (options.CurrentValue.SkipSignedAssemblies && assembly.GetName().GetPublicKeyToken()?.Length > 0) return;
        if (scannedAssemblies.Contains(assembly.FullName)) return;
        scannedAssemblies.Add(assembly.FullName);

        var index = assembly.FullName.IndexOf('.');

        var firstSegment = index >= 0 ? assembly.FullName.Substring(0, index) : assembly.FullName;

        if (options.CurrentValue.PrefixWhitelist.Count > 0 && !options.CurrentValue.PrefixWhitelist.Contains(firstSegment)) return;

        string path = assembly.Location;
        DateTime? lastModified = null;
        if (File.Exists(path))
        {
            lastModified = System.IO.File.GetLastWriteTime(path);
        }
        if (lastModified.HasValue)
        {
            list.Add(lastModified.Value, assembly);
        }
        else
        {
            logger.LogWarning("Could not get date for assembly: {assembly}", assembly.FullName);
        }
        foreach (var a in assembly.GetReferencedAssemblies())
        {
            Recurse(Assembly.Load(a), list, depth + 1);
        }
    }

    HashSet<string> scannedAssemblies = new();

    private void LogAssembly(StringBuilder sb, Assembly assembly)
    {

        string path = assembly.Location;
        DateTime? lastModified = null;
        if (File.Exists(path))
        {
            lastModified = System.IO.File.GetLastWriteTime(path);
        }
        sb.AppendLine($"{assembly.GetName().Name.PadRight(50)} {assembly.GetName().Version.ToString().PadRight(12)} {(lastModified?.ToString() ?? "unknown date")}");
    }


    public Task StartAsync(CancellationToken cancellationToken)
    {
        DoLog();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
