namespace LionFire.Data
{
    public static class ITransferX
    {        
        public static bool IsSuccess(this TransferResultFlags flags) => flags.HasFlag(TransferResultFlags.Success) || flags.HasFlag(TransferResultFlags.PreviewSuccess);
        public static bool? IsSuccessTernary(this TransferResultFlags flags)
        {
            if (flags.HasFlag(TransferResultFlags.Success) || flags.HasFlag(TransferResultFlags.PreviewSuccess)) return true;
            if (flags.HasFlag(TransferResultFlags.Fail) || flags.HasFlag(TransferResultFlags.PreviewFail)) return false;
            /// if (flags.HasFlag(TransferResultFlags.PreviewIndeterminate)) return null; // Redundant
            return null;
        }

    }
}