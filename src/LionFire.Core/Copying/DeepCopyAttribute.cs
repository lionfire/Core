using System;

namespace LionFire.Copying
{
    /// <summary>
    /// Include non-public member in DeepCopy even if non-public members are excluded by default
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
    public sealed class DeepCopyAttribute : Attribute
    {
    }

}
