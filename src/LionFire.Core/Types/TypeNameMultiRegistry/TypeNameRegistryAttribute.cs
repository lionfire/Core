namespace LionFire.Types;

[System.AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = true, AllowMultiple = false)]
public sealed class TypeNameRegistryAttribute : Attribute
{
    public TypeNameRegistryAttribute(string name)
    {
        Name = name;
    }

    public string Name { get; }
}