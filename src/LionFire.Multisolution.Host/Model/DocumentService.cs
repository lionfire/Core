using LionFire.Threading;
using Microsoft.Extensions.Options;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using NuGet.Common;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Text.Json;
using Mono.TextTemplating.CodeCompilation;
using static System.Console;
using System.Xml.Linq;

namespace LionFire.MultiSolution.Host.Model;

public class DocumentsOptions
{
    public bool LoadMostRecent { get; set; }
    public List<string> MostRecent { get; set; }

}

public class DocumentService : ReactiveObject
{
    [Reactive]
    public MultiSolutionDocument Document { get; set; } = new();

    [Reactive]
    public string? DocumentPath { get; set; }

    public HashSet<string> CurrentPrerelease { get; set; } = new();

    public IOptionsMonitor<DocumentsOptions> OptionsMonitor { get; }
    public ILogger<DocumentService> Logger { get; }

    public DocumentsOptions Options => OptionsMonitor.CurrentValue;

    public DocumentService(IOptionsMonitor<DocumentsOptions> optionsMonitor, ILogger<DocumentService> logger)
    {
        //Logger.LogInformation("DocumentService initializing");
        OptionsMonitor = optionsMonitor;
        Logger = logger;
        DoEverything().FireAndForget();

    }

    private async Task<(string? release, string? prerelease)> GetLatestVersion(string packageId)
    {
        var logger = NullLogger.Instance;
        CancellationToken cancellationToken = CancellationToken.None;

        SourceCacheContext cache = new SourceCacheContext();
        SourceRepository repository = Repository.Factory.GetCoreV3("https://api.nuget.org/v3/index.json");
        FindPackageByIdResource resource = await repository.GetResourceAsync<FindPackageByIdResource>();

        IEnumerable<NuGetVersion> versions = await resource.GetAllVersionsAsync(
            packageId,
            cache,
            logger,
            cancellationToken);

        SemVersion? highestRelease = null; // TODO: Use NuGet.Versioning instead
        SemVersion? highestPrerelease = null;

        foreach (NuGetVersion version in versions)
        {
            if (SemVersion.TryParse(version.ToString(), out var current))
            {
                if (!version.IsPrerelease && (highestRelease == null || current > highestRelease))
                {
                    highestRelease = current;
                }
                if (highestPrerelease == null || current > highestPrerelease.Value
                    || (current.Major == highestPrerelease.Value.Major
                    && current.Minor == highestPrerelease.Value.Minor
                    && current.Patch == highestPrerelease.Value.Patch
                    && !current.IsPreRelease && highestPrerelease.Value.IsPreRelease
                    )
                    )
                {
                    highestPrerelease = current;
                }
            }
        }
        WriteLine($"[nuget.org] {packageId}: {highestRelease?.ToString()},  {(highestPrerelease?.ToString() == highestRelease?.ToString() ? "" : highestPrerelease?.ToString())}");
        return (highestRelease?.ToString(), highestPrerelease?.ToString());
    }

    public Task Upgrade(bool pretend = true, bool major = false, bool minor = true, bool consolidateOnly = false, string? singlePackageId = null, bool prerelease = false)
    {
        var d = Document;

        var propsDocs = new Dictionary<string, XDocument>();
        var propsDocsDirty = new Dictionary<string, XDocument>();

        foreach (var path in d.DirectoryPackagesProps)
        {
            XDocument doc = XDocument.Load(path);
            propsDocs.Add(path, doc);

            if (doc.Root == null) continue;
            var packageVersions = doc.Root.Nodes().OfType<XElement>().Where(n => n.Name == "ItemGroup").SelectMany(ig => ig.Nodes().OfType<XElement>().Where(n => n.Name == "PackageVersion"));

            foreach (var packageVersion in packageVersions)
            {
                var include = packageVersion.Attribute("Include")?.Value;
                var version = packageVersion.Attribute("Version")?.Value;

                if (include == null || version == null)
                {
                    continue;
                }
            }
        }

        foreach (var kvp in d.CurrentPackageVersions
            //.Where(x => x.Value.StartsWith("("))
            )
        {
            var packageId = kvp.Key;
            var currentVersion = kvp.Value;
            if (singlePackageId != null && packageId != singlePackageId) continue;
            //if (!packageId.StartsWith("Microsoft.") && !packageId.StartsWith("System.")) continue;

            if (d.IgnoredPackages.Contains(packageId)) continue;

            var available = (prerelease || CurrentPrerelease.Contains(kvp.Key)) 
                ? d.AvailablePrereleasePackageVersions 
                : d.AvailablePackageVersions;

            if (available.TryGetValue(kvp.Key, out var availableVersion))
            {
                if (string.IsNullOrWhiteSpace(availableVersion) || availableVersion == currentVersion) continue;

                //WriteLine($"[UPGRADING] {kvp.Key} to {availableVersion}");
            }

            foreach (var propsDoc in propsDocs)
            {
                var packageVersion = propsDoc.Value.Root.Nodes().OfType<XElement>().Where(n => n.Name == "ItemGroup").SelectMany(ig => ig.Nodes().OfType<XElement>().Where(n => n.Name == "PackageVersion"
                && n.Attribute("Include")?.Value == packageId
                )).FirstOrDefault();

                if (packageVersion == null) continue;

                var displayKey = propsDoc.Key.Replace(@"C:\\src\\","").Replace("\\Directory.Packages.props", "");

                if (SemVersion.TryParse(packageVersion.Attribute("Version").Value, out var currentSemVersion)
                    && SemVersion.TryParse(availableVersion, out var availableSemVersion)
                    )
                {
                    if (consolidateOnly && currentVersion != "(multiple versions)") continue;

                    if (currentSemVersion.ToString() == availableSemVersion.ToString()) continue;
                    if (currentSemVersion > availableSemVersion)
                    {
                        WriteLine($"skipping (downgrade) - {displayKey}: {packageId} '{packageVersion.Attribute("Version").Value}' ==> '{availableVersion}'");
                        continue;
                    }
                    if (!major && currentSemVersion.Major != availableSemVersion.Major)
                    {
                        WriteLine($"skipping (major disabled) - {displayKey}: {packageId} '{packageVersion.Attribute("Version").Value}' ==> '{availableVersion}'");
                        continue;
                    }
                    if (!minor && currentSemVersion.Minor != availableSemVersion.Minor)
                    {
                        WriteLine($"skipping (minor disabled) - {displayKey}: {packageId} '{packageVersion.Attribute("Version").Value}' ==> '{availableVersion}'");
                        continue;
                    }

                    WriteLine($"{packageId}: '{packageVersion.Attribute("Version").Value}' ==> '{availableVersion}' ({displayKey})");
                    if (!pretend)
                    {
                        packageVersion.Attribute("Version")!.Value = availableVersion;
                        if (!propsDocsDirty.ContainsKey(propsDoc.Key)) { propsDocsDirty.Add(propsDoc.Key, propsDoc.Value); }
                    }
                }
            }
        }

        foreach (var kvp in propsDocsDirty)
        {
            kvp.Value.Save(kvp.Key);
        }

        return Task.CompletedTask;
    }

    private async Task DoEverything()
    {
        await TryLoadMostRecent();
        await ScanNuget();
    }

    List<string> LogList = new();
    private void Log(string msg)
    {
        Console.WriteLine(msg);
        LogList.Add(msg);
    }

    //static int MaxNugetRetrieves = 1000;
    public async Task ScanNuget(bool forceRescan = false)
    {
        var rescan = forceRescan || Document.LastCheckedNuget + Document.NugetRescanInterval < DateTime.UtcNow;

        Dictionary<string, string>? oldVersions = null;
        Dictionary<string, string>? oldPrereleaseVersions = null;

        if (rescan)
        {
            oldVersions = Document.AvailablePackageVersions;
            oldPrereleaseVersions = Document.AvailablePrereleasePackageVersions;

            Document.AvailablePackageVersions = new Dictionary<string, string>();
            Document.AvailablePackageVersions = new Dictionary<string, string>();
        }

        try
        {
            foreach (var kvp in Document.CurrentPackageVersions)
            {
                if (!Document.AvailablePackageVersions.TryGetValue(kvp.Key, out var _)
                    || !Document.AvailablePrereleasePackageVersions.TryGetValue(kvp.Key, out var _))
                {
                    //if (MaxNugetRetrieves-- < 0)
                    //{
                    //    Console.WriteLine("MaxNugetRetrieves reached");
                    //    break;
                    //}
                    var result = await GetLatestVersion(kvp.Key);

                    if (!Document.AvailablePackageVersions.TryGetValue(kvp.Key, out var _))
                    {
                        if (oldVersions != null && oldVersions.ContainsKey(kvp.Key) && oldVersions[kvp.Key] != (result.release ?? ""))
                        {
                            Log($"{kvp.Key} release: \"{oldVersions[kvp.Key]}\" ==> \"{result.release}\"");
                        }

                        Document.AvailablePackageVersions.Add(kvp.Key, result.release ?? "");
                    }
                    if (!Document.AvailablePrereleasePackageVersions.TryGetValue(kvp.Key, out var _))
                    {
                        if (oldPrereleaseVersions != null && oldPrereleaseVersions.ContainsKey(kvp.Key) && oldPrereleaseVersions[kvp.Key] != (result.release ?? ""))
                        {
                            Log($"{kvp.Key} prerelease: \"{oldPrereleaseVersions[kvp.Key]}\" ==> \"{result.prerelease}\"");
                        }

                        Document.AvailablePrereleasePackageVersions.Add(kvp.Key, result.prerelease ?? "");
                    }
                }
            }

            if (rescan)
            {
                Console.WriteLine("Rescanned from nuget.");
                Document.LastCheckedNuget = DateTime.UtcNow;
            }
        }
        finally
        {
            await Save();
        }
    }

    public async Task<bool> TryLoadMostRecent()
    {
        if (!Options.LoadMostRecent) return false;

        var path = Options?.MostRecent.FirstOrDefault();
        if (string.IsNullOrEmpty(path)) return false;
        return await TryLoad(path);
    }

    public async Task<bool> TryLoad(string? path = null)
    {
        path ??= DocumentPath;
        if(path == null) throw new ArgumentNullException($"{nameof(path)} or {nameof(DocumentPath)} must be set");

        var json = File.ReadAllText(path);
        var doc = JsonSerializer.Deserialize<MultiSolutionDocument>(json);


        DocumentPath = doc == null ? null : path;
        if (doc != null)
        {
            Document = doc;
            await doc.Load();
        }
        return doc != null;
    }

    public Task Save(string? path = null)
    {
        path ??= DocumentPath;
        if (path == null) throw new ArgumentNullException($"{nameof(Path)} or {nameof(DocumentPath)} must be set");

        var json = JsonSerializer.Serialize<MultiSolutionDocument>(Document, new JsonSerializerOptions()
        {
            WriteIndented = true
        });
        File.WriteAllText(path, json);
        return Task.CompletedTask;
    }
}
