using LionFire.Referencing;

namespace LionFire.Persistence.Assets
{
    public interface ISubPathReference : IReference
    {
        string SubPath { get; }
    }
}
