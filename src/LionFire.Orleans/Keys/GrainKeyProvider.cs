using Orleans;

namespace LionFire.Structures.Keys;

public class GrainKeyProvider : IKeyProvider<string>
{
    public (bool success, string? key) TryGetKey(object obj)
    {
        var grain = obj as IGrainWithStringKey;
        if (grain == null) return (false, null);
        var key = grain.GetPrimaryKeyString();
        return (true, key);
    }
}
