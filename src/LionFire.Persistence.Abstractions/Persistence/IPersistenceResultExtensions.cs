namespace LionFire.Persistence
{
    public static class IPersistenceResultExtensions
    {
        public static bool IsError(this IPersistenceResult result) => result.Kind.HasFlag(PersistenceResultKind.Error);

        public static bool IsRetrieved(this IPersistenceResult result) => result.Kind.HasFlag(PersistenceResultKind.Retrieved);

        public static bool IsFound(this IPersistenceResult result) => result.Kind.HasFlag(PersistenceResultKind.Found);
        public static bool IsSuccess(this IPersistenceResult result) => result.Kind.HasFlag(PersistenceResultKind.Success);
    }

}