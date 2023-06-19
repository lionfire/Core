using LionFire.Results;

namespace LionFire.Data; // MOVE to LionFire.Data


// GLOBALRENAME to ITransferResult
public interface IPersistenceResult : IErrorResult, ISuccessResult, INoopResult
{
    PersistenceResultFlags Flags { get; set; }
}