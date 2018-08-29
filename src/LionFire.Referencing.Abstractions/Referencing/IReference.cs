using LionFire.Structures;

namespace LionFire.Referencing
{
    public interface IReference : IKeyed<string>, IMachineReference
    {
        string Scheme { get; }
        string Path { get; }
    }
}
