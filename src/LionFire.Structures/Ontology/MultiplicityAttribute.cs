using System;

namespace LionFire.Structures
{

    [System.AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class MultiplicityAttribute : Attribute
    {
        public MultiplicityAttribute(Multiplicity multiplicity)
        {
            Multiplicity = multiplicity;
        }

        public Multiplicity Multiplicity { get; }
    }
}
