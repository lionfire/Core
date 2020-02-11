using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Ontology
{
    [System.AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]
    public sealed class TreatAsAttribute : Attribute
    {
        public TreatAsAttribute(Type type)
        {
            Type = type;
        }

        public Type Type { get; }

        public object Context { get; set; }
    }
}
