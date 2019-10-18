using LionFire.Results;

namespace LionFire.Persistence
{

    public interface IPersistenceResult : IErrorResult
    {
        PersistenceResultFlags Flags { get; set; }
    }

}