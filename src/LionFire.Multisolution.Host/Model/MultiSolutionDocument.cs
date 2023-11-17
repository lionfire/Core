using ReactiveUI;
using net.r_eg.MvsSln;
using static System.Console;
using System.Xml;
using System.Xml.Linq;
using System.Linq;

namespace LionFire.MultiSolution.Host.Model;

public class MultiSolutionDocument : ReactiveObject
{
    public List<string> Solutions { get; set; } = new();

    public List<string> DirectoryPackagesProps { get; set; } = new();

    public DateTime LastCheckedNuget { get; set; } = DateTime.MinValue;
    public TimeSpan NugetRescanInterval { get; set; } = TimeSpan.FromHours(24);

    public Dictionary<string, string> CurrentPackageVersions { get; set; } = new();
    public Dictionary<string, string> AvailablePackageVersions { get; set; } = new();
    public Dictionary<string, string> AvailablePrereleasePackageVersions { get; set; } = new();

    public async Task Load()
    {
        //await GetDirectoryPackagesProps();
        await LoadDirectoryPackagesProps();
    }

    public Task LoadDirectoryPackagesProps()
    {
        var dict = new Dictionary<string, string>();

        foreach(var path in DirectoryPackagesProps)
        {
            XDocument doc = XDocument.Load(path);
            if (doc.Root == null) continue;
            var packageVersions = doc.Root.Nodes().OfType<XElement>().Where(n => n.Name == "ItemGroup").SelectMany(ig => ig.Nodes().OfType<XElement>().Where(n => n.Name == "PackageVersion"));

            foreach(var packageVersion in packageVersions)
            {
                var include = packageVersion.Attribute("Include")?.Value;
                var version = packageVersion.Attribute("Version")?.Value;

                if (include == null || version == null)
                {
                    continue;
                }
                if (!dict.ContainsKey(include))
                {
                    dict.Add(include, version);
                }
                else if (dict[include] != version)
                {
                    dict[include] = "(multiple versions)";
                }
            }
        }

        CurrentPackageVersions = dict;
        return Task.CompletedTask;
    }

    public Task GetDirectoryPackagesProps()
    {
        foreach (var slnPath in Solutions)
        {
            using var sln = new Sln(slnPath,
                SlnItems.SolutionItems
                |SlnItems.AllMinimal
                //SlnItems.All & ~SlnItems.ProjectDependencies
                ); 

            foreach(var slnItem in sln.Result.SolutionFolders.SelectMany(sf => sf.items))
            {
                WriteLine(slnItem);
            }
#if TODO
            //sln.Result.Env.XProjectByGuid(
            //    sln.Result.ProjectDependencies.FirstBy(BuildType.Rebuild).pGuid,
            //    new ConfigItem("Debug", "Any CPU")
            //);

            var p = sln.Result.Env.GetOrLoadProject(
                sln.ProjectItems.FirstOrDefault(p => p.name == name)
            );

            var paths = sln.Result.ProjectItems
                                    .Select(p => new { p.pGuid, p.fullPath })
                                    .ToDictionary(p => p.pGuid, p => p.fullPath);

            // {[{27152FD4-7B94-4AF0-A7ED-BE7E7A196D57}, D:\projects\Conari\Conari\Conari.csproj]}
            // {[{0AEEC49E-07A5-4A55-9673-9346C3A7BC03}, D:\projects\Conari\ConariTest\ConariTest.csproj]}

            foreach (IXProject xp in sln.Result.Env.Projects)
            {
                xp.AddReference(typeof(JsonConverter).Assembly, true);
                xp.AddReference("System.Core");

                ProjectItem prj = ...
        xp.AddProjectReference(prj);

                xp.AddImport("../packages/DllExport.1.5.1/tools/net.r_eg.DllExport.targets", true);

                xp.SetProperty("JsonConverter", "30ad4fe6b2a6aeed", "'$(Configuration)' == 'Debug'");
                xp.SetProperties
                (
                    new[]
                    {
                new PropertyItem("IsCrossTargetingBuild", "true"),
                new PropertyItem("CSharpTargetsPath", "$(MSBToolsLocal)\\CrossTargeting.targets")
                    },
                    "!Exists('$(MSBuildToolsPath)\\Microsoft.CSharp.targets')"
                );
                // ...
            }

            sln.Result.ProjectConfigs.Where(c => c.Sln.Configuration == "Debug"); // project-cfgs by solution-cfgs
                                                                                  // ...
#endif

        }
        return Task.CompletedTask;
    }
}
