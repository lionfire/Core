using LionFire.Results;

namespace LionFire.Persistence
{
    public interface IPersistenceResult : IErrorResult, ISuccessResult
    {
        PersistenceResultFlags Flags { get; set; }
    }

}