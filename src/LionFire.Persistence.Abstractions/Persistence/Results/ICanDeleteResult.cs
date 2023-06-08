using LionFire.Data.Async.Gets;

namespace LionFire.Persistence
{
    //public interface ICanDeleteResult : IPersistenceResult
    //{
    //    //bool? CanDelete { get; }
    //}
    public static class ICanDeleteResultExtensions
    {
        public static bool? CanDelete(this IPersistenceResult result) => result.IsPreviewSuccessTernary();
    }
    
}