using System;

namespace LionFire.Ontology;

[System.AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
public sealed class MutableAttribute : Attribute
{
}
