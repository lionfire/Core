using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Ontology
{
    /// <summary>
    /// Indicates that the specified class, property, field, is immutable and that it should not be possible to change the value after creation.
    /// On methods, indicates that the method does not mutate this.  On method parameters, indicates the method does not mutate the related parameter.
    /// </summary>
    [System.AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public sealed class ImmutableAttribute : Attribute
    {
        
    }
}
