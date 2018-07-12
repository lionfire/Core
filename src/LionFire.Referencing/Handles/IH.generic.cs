using LionFire.Structures;

namespace LionFire.Referencing
{
    public interface IH<ObjectType> : IReadHandle<ObjectType>, IKeyed<string>, IReferencable
        where ObjectType : class
    {
    }
}
