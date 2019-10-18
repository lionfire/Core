using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Ontology
{
    [System.AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public sealed class CastsAttribute : Attribute
    {
        public CastsAttribute(string castDescription, Type type = null) { CastDescription = castDescription; this.Type = type; }

        public string CastDescription { get; }
        public Type Type { get; }
    }
}
