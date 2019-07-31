using System;

namespace LionFire.Persistence
{
    // TODO: sort out nomenclature for Read vs Retrieve, propagate these interfaces to the right places

    [Flags]
    public enum PersistenceResultKind
    {
        Unspecified = 0,

        /// <summary>
        /// True if operation was successful, even if retrieve did not find anything
        /// </summary>
        Success = 1 << 0,

        Error = 1 << 1,

        Found = 1 << 2,
        NotFound = 1 << 3,

        Retrieved = 1 << 4,
    }

}