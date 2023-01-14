namespace LionFire.Persistence
{
    public class PersisterRetryOptions
    {
        public static PersisterRetryOptions Default { get; } = new();

        // REVIEW - is there a more pluggable way of getting these?  Make this type MultiTypable and have named Decorators for AutoRetry for Retrieve/Put/etc.?
        public int MaxGetRetries { get; set; } = 5;
        public int MaxDeleteRetries { get; set; } = 3;
        public int MillisecondsBetweenGetRetries { get; set; } = 75;
        public int MillisecondsBetweenDeleteRetries { get; set; } = 150;
    }
}
