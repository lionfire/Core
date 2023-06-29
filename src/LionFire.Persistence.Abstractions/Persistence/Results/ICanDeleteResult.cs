using LionFire.Data.Gets;

namespace LionFire.Persistence
{
    //public interface ICanDeleteResult : ITransferResult
    //{
    //    //bool? CanDelete { get; }
    //}
    public static class ICanDeleteResultExtensions
    {
        public static bool? CanDelete(this ITransferResult result) => result.IsPreviewSuccessTernary();
    }
    
}