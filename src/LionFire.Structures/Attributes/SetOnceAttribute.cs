using System;

namespace LionFire
{
    /// <summary>
    /// When defined on a class, properties with the class as the type should not be set more than once
    /// </summary>
    [System.AttributeUsage(AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public sealed class SetOnceAttribute : Attribute
    {
        
    }
}
