using LionFire.Structures;

namespace LionFire.Referencing
{
    /// <summary>
    /// Reference interface for all references: OBase, Vos, Handles.
    /// References should be round-trip convertible to and from a string key (from IKeyed)
    /// </summary>
    public interface IReference : IKeyed<string>, IMachineReference
    {
        string Scheme { get; }
        string Path { get; }
    }

    public interface IReference<T>
    {
        IReference Reference { get; }
    }

}
