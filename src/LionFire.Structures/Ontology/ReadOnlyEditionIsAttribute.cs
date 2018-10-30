using System;

namespace LionFire.Ontology
{
    [System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class ReadOnlyEditionIsAttribute : Attribute
    {
        public Type Type { get; private set; }
        public ReadOnlyEditionIsAttribute(Type type) { this.Type = type; }
    }
}
