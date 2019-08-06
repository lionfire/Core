namespace LionFire.Persistence
{
    public static class IPersistenceResultExtensions
    {
        public static bool IsFail(this IPersistenceResult result) => result.Flags.HasFlag(PersistenceResultFlags.Fail);

        public static bool IsRetrieved(this IPersistenceResult result) => result.Flags.HasFlag(PersistenceResultFlags.Retrieved);

        public static bool IsFound(this IPersistenceResult result) => result.Flags.HasFlag(PersistenceResultFlags.Found);
        public static bool IsSuccess(this IPersistenceResult result) => result.Flags.HasFlag(PersistenceResultFlags.Success);

        public static void ThrowIfUnsuccessful(this IPersistenceResult result)
        {
            if (!result.IsSuccess()) throw new PersistenceException(result);
        }
    }

}