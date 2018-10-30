using System;

namespace LionFire.Copying
{
    /// <summary>
    /// Include non-public member in DeepCopy even if non-public members are excluded by default
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
    public sealed class DeepCopyFlagsAttribute : Attribute
    {
        public CopyFlags CopyFlags { get; }

        public DeepCopyFlagsAttribute(CopyFlags copyFlags)
        {
            this.CopyFlags = copyFlags;
        }
    }

}
