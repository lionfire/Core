using System.Collections.Generic;
using System.Linq;

namespace LionFire.Deployment;

public static class ReleaseChannels
{
    public static IEnumerable<ReleaseChannel> ProdChannels
    {
        get
        {
            yield return Prod;
            yield return Green;
            yield return Blue;
        }
    }

    public static IEnumerable<ReleaseChannel> All
    {
        get
        {
            yield return Prod;
            yield return Green;
            yield return Blue;

            yield return Beta;
            yield return Alpha;

            yield return Nightly;
            yield return AutoBuild;

            yield return Test;

            yield return Dev;
            yield return Local;
            yield return Src;
        }
    }

    public static ReleaseChannel? TryParse(string? id)
    {
        id = id?.ToKebabCase();

        if (id == null) return null;
        //if (Dictionary.TryGetValue(id, out ReleaseChannel? value)) return value;
        if (KebabDictionary.TryGetValue(id, out ReleaseChannel? value2)) return value2;

        return null;
    }
    public static ReleaseChannel Parse(string id) => TryParse(id) ?? new ReleaseChannel(id, id, 0);

    public static IDictionary<string, ReleaseChannel> Dictionary => dictionary ??= All.ToDictionary(x => x.Id);
    private static IDictionary<string, ReleaseChannel>? dictionary;
    public static IDictionary<string, ReleaseChannel> KebabDictionary => kebabDictionary ??= All.ToDictionary(x => x.KebabCaseId);
    private static IDictionary<string, ReleaseChannel>? kebabDictionary;

    public static ReleaseChannel Prod = new("Prod", "Production", 0);
    public static ReleaseChannel Green = new("Green", "Production", 0);
    public static ReleaseChannel Blue = new("Blue", "Production", 1);

    public static ReleaseChannel Beta = new ("Beta", "Beta", 2);
    public static ReleaseChannel Alpha = new ("Alpha", "Alpha", 3);
    public static ReleaseChannel Nightly = new ("Nightly", "Nightly", 4);
    public static ReleaseChannel AutoBuild = new ("AutoBuild", "AutoBuild", 5);
    public static ReleaseChannel Test = new("Test", "Test", 6);
    public static ReleaseChannel Dev = new("Dev", "Development", 7);
    public static ReleaseChannel Local = new("Local", "Local Development", 8);
    public static ReleaseChannel Src = new("Src", "Source", 9);

    public static ReleaseChannel InProcess = new("InProcess", "In process, for Local Development", 10);

    public static int? TryGetReleaseChannelPortOffset(string id)
        => All.Where(r => r.Id == id).FirstOrDefault()?.Precedence;

    //public static Dictionary<string, int> ReleaseChannelPortOffsets = new()
    //{
    //    ["prod"] = 0,
    //    ["green"] = 0,
    //    ["blue"] = 1,

    //    ["beta"] = 2,
    //    ["alpha"] = 3,
    //    //["gamma"] = 3,
    //    //["test"] = 0,
    //    ["dev"] = 6,
    //};
}
