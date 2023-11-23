using LionFire.Persistence;

namespace LionFire.Persistence;

public static class IPersistsX
{
    public static bool NotFound(this IPersists has) => has.Flags.HasFlag(PersistenceFlags.NotFound);
    public static bool RetrievedNullOrDefault(this IPersists has) => has.Flags.HasFlag(PersistenceFlags.UpToDate);

}
