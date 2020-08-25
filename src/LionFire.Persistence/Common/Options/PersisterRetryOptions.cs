namespace LionFire.Persistence
{
    public class PersisterRetryOptions
    {
        // REVIEW - is there a more pluggable way of getting these?  Make this type MultiTypable and have named Decorators for AutoRetry for Retrieve/Put/etc.?
        public int MaxGetRetries { get; set; } = 0;
        public int MaxDeleteRetries { get; set; } = 0;
        public int MillisecondsBetweenGetRetries { get; set; } = 500;
        public int MillisecondsBetweenDeleteRetries { get; set; } = 500;
    }
}
