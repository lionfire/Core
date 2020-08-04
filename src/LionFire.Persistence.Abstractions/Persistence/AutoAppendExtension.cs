using System;

namespace LionFire.Persistence
{
    [Flags]
    public enum AutoAppendExtension
    {
        /// <summary>
        /// (Default)
        /// </summary>
        Disabled = 0,
        IfNoExtensions = 1 << 0,
        IfIncorrectExtension = 1 << 1,

        ///// <summary>
        ///// FUTURE (Recommended)
        ///// </summary>
        //Always,
    }

    public enum AppendExtensionOnRead
    {
        /// <summary>
        /// (Default) Require retrieve requests to use exact path including extension
        /// </summary>
        Never = 0,

        /// <summary>
        /// (Recommended) Require retrieve requests to not specify an extension.  Installed serializers will be attempted in priority order for present files that have relevant file extensions.
        /// </summary>
        Always = 1,

        /// <summary>
        /// Optional: if an extension is provided, don't attempt to append extensions.
        /// </summary>
        IfNoExtensions = 2,

        // FUTURE: like IfNoExtensions but if that fails, try all serializers on all potential files.
        //IfIncorrectExtension = 1 << 1,
    }

    // TODO - FUTURE
    [Flags]
    public enum ValidateOneFilePerPath
    {
        None = 0,

        /// <summary>
        /// When retrieving a path, throw an exception if there is more than one file name with different extensions (including no extension).
        /// </summary>
        OnRead = 1 << 0,
        OnWrite = 1 << 1,

        /// <summary>
        /// When mounting ensure all directories do not contain any files that differ only by extension.
        /// </summary>
        AtStartup = 1 << 2,
    }
}
