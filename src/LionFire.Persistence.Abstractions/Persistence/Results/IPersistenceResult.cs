using LionFire.Resolves;
using LionFire.Results;

namespace LionFire.Persistence
{
    // TODO: Migrate IRetrieveResult<T> to this?
    //public interface IPersistenceResult<out T> : IPersistenceResult, IValueResult<T>, IHasValueResult
    //{
    //}
    public interface IPersistenceResult : IErrorResult, ISuccessResult, INoopResult
    {
        PersistenceResultFlags Flags { get; set; }
    }

}