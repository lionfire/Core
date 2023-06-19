namespace LionFire.Persistence
{
    public static class ITransferResultExtensions
    {
        public static bool IsFail(this ITransferResult result) => result.Flags.HasFlag(TransferResultFlags.Fail);

        public static bool IsRetrieved(this ITransferResult result) => result.Flags.HasFlag(TransferResultFlags.Retrieved);

        public static bool? IsFound(this ITransferResult result) => result.Flags.IsFound();
        public static bool IsSuccess(this ITransferResult result) => result.Flags.HasFlag(TransferResultFlags.Success);
        public static bool IsPreviewSuccess(this ITransferResult result) => result.Flags.HasFlag(TransferResultFlags.PreviewSuccess);
        public static bool? IsPreviewSuccessTernary(this ITransferResult result)
        {
            if (result.Flags.HasFlag(TransferResultFlags.PreviewSuccess)) return true;
            if (result.Flags.HasFlag(TransferResultFlags.PreviewFail)) return false;
            return null;
        }

        public static T ThrowIfUnsuccessful<T>(this T result)
            where T : ITransferResult
        {
            if (!result.IsSuccess()) throw new PersistenceException(result);
            return result;
        }
    }
}