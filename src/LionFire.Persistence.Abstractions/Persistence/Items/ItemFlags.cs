#nullable enable

using System;

namespace LionFire.Persistence
{

    [Flags]
    public enum ItemFlags
    {
        None = 0,
        File = 1 << 0,
        Directory = 1 << 1,
        Hidden = 1 << 2,
        Meta = 1 << 3,
        Special = 1 << 4,
        Default = File | Directory,
        All = File | Directory | Hidden | Meta | Special,
    }
}
