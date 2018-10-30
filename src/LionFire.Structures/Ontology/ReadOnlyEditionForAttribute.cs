using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Ontology
{
    [System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class ReadOnlyEditionForAttribute : Attribute
    {
        public Type Type { get; private set; }
        public ReadOnlyEditionForAttribute(Type type) { this.Type = type; }
    }
}
